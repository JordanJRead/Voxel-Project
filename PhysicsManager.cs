using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal static class PhysicsManager
    {
        public static bool SquareOverlap(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
        {
            return !(aMax.X < bMin.X || aMin.X > bMax.X || aMax.Y < bMin.Y || aMin.Y > bMax.Y);
        }

        /// <summary>
        /// Axis aligned bounding box
        /// </summary>
        class AABB
        {
            public Vector3 min;
            public Vector3 max;

            public AABB(Vector3 p1, Vector3 p2)
            {
                this.min = new Vector3(MathF.Min(p1.X, p2.X), MathF.Min(p1.Y, p2.Y), MathF.Min(p1.Z, p2.Z));
                this.max = new Vector3(MathF.Max(p1.X, p2.X), MathF.Max(p1.Y, p2.Y), MathF.Max(p1.Z, p2.Z));
            }

            public static AABB FromPositionAndScale(Vector3 position, Vector3 scale)
            {
                return new AABB(position - scale * 0.5f, position + scale * 0.5f);
            }

            public Vector3 GetCenter()
            {
                return min + (max - min) * 0.5f;
            }

            /// <summary>
            /// Returns a new AABB that is the same as the current, but moved
            /// </summary>
            /// <param name="moveBy"></param>
            public AABB NewMoveBy(Vector3 moveBy)
            {
                AABB newAABB = new AABB(min, max);
                newAABB.min += moveBy;
                newAABB.max += moveBy;
                return newAABB;
            }
            public bool Intersects(AABB other)
            {
                return !(this.min.X >= other.max.X || this.max.X <= other.min.X || this.min.Y >= other.max.Y || this.max.Y <= other.min.Y || this.min.Z >= other.max.Z || this.max.Z <= other.min.Z);
            }

            /// <summary>
            /// Returns if the two AABBs overlap when viewed from an axis
            /// </summary>
            /// <param name="dim">Either x, y, or z. The axis to view the AABBs from</param>
            /// <returns>Whether the AABBs overlap, or false, if dim is not in {'x', 'y', 'z'}</returns>
            public bool DoSquaresIntersect(AABB other, char dim)
            {
                dim = dim.ToString().ToLower().ToCharArray()[0];
                switch (dim)
                {
                    case 'x':
                        return SquareOverlap(this.min.Yz, this.max.Yz, other.min.Yz, other.max.Yz);
                    case 'y':
                        return SquareOverlap(this.min.Xz, this.max.Xz, other.min.Xz, other.max.Xz);
                    case 'z':
                        return SquareOverlap(this.min.Xy, this.max.Xy, other.min.Xy, other.max.Xy);
                    default:
                        throw new InvalidOperationException("dim is not one of {'x', 'y', 'z'}");
                }
            }

            public static AABB Merge(AABB a, AABB b)
            {
                return new AABB(
                    p1: new Vector3(Math.Min(a.min.X, b.min.X), Math.Min(a.min.Y, b.min.Y), Math.Min(a.min.Z, b.min.Z)),
                    p2: new Vector3(Math.Max(a.max.X, b.max.X), Math.Max(a.max.Y, b.max.Y), Math.Max(a.max.Z, b.max.Z))
                    );
            }
        }

        /// <summary>
        /// Decides where the player will move in a frame
        /// </summary>
        /// <param name="displacement">The movement vector that the player wants to move by</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns></returns>
        public static Vector3 MoveInScene(PlayerController player, Scene scene, Vector3 displacement, int depth = 0, int maxDepth = 20)
        {
            if (depth == maxDepth)
                return player.GetPosition() + displacement;
            /*
            THE ALGORITHM:
            - Check each axis for the closest collision t-value (0-1)
                - For each axis, check where the scene object's N-value lies between the player's starting N and ending N (N in {x, y, z})
                - Find t-value with (between - playerStart.N) / (playerEnd.N - playerStart.N)
                - Ensure that when the palyer AABB is at the collision, they are touching (check if the square formed in the N-plane overlap)
                - If between 0 and 1, save it
                - (Will have to decide if player is moving in + or - N to decide whether to use min/max
            - If there's an intersection, move the player along displacement by the closest t
            - Set their new displacement to displacement (where N is 0) * (1 - closestT)
            - Call again

            I did look at https://gamedev.stackexchange.com/questions/164774/aabb-to-aabb-collision-response
            but all the code is mine, and most of the algorithms I though of myself
             */
            Vector3 playerCenter = player.GetPosition();
            Vector3 desiredPlayerCenter = playerCenter + displacement;

            AABB playerAABB = AABB.FromPositionAndScale(playerCenter, player.GetSize());
            AABB desiredPlayerAABB = AABB.FromPositionAndScale(desiredPlayerCenter, player.GetSize());
            AABB mergedAABB = AABB.Merge(desiredPlayerAABB, playerAABB);

            List<AABB> possibleCollisionAABBs = new List<AABB>();

            foreach (Voxel voxel in scene.GetVoxels())
            {
                AABB voxelAABB = AABB.FromPositionAndScale(voxel.GetPosition(), new Vector3(0.5f));
                if (mergedAABB.Intersects(voxelAABB))
                {
                    possibleCollisionAABBs.Add(voxelAABB);
                }
            }

            foreach (Fence fence in scene.GetFences())
            {
                AABB postAABB = AABB.FromPositionAndScale(fence.GetPosition(), new Vector3(0.1f, 0.5f, 0.1f));
                if (mergedAABB.Intersects(postAABB))
                {
                    possibleCollisionAABBs.Add(postAABB);
                }

                //if (fence.GetConnection(Fence.ConnectionType.posX))
                //{
                //    AABB connectionAABB = new AABB(fence.GetPosition() + new Vector3(0.25f) - new Vector3(0.25f, 0.05f, 0.05f), fence.GetPosition() + new Vector3(0.25f, 0.05f, 0.05f);
                //    if (mergedAABB.Intersects(connectionAABB))
                //    {
                //        possibleCollisionAABBs.Add(connectionAABB);
                //    }
                //}
            }

            possibleCollisionAABBs.Sort(new Comparison<AABB>((AABB aabb1, AABB aabb2) =>
            {
                float dist1 = (aabb1.GetCenter() - playerCenter).Length;
                float dist2 = (aabb2.GetCenter() - playerCenter).Length;

                return dist1 < dist2 ? -1 : 1;
            }));

            foreach (AABB aabb in possibleCollisionAABBs)
            {
                // The motion sign in each dimension
                float[] signDim =
                {
                    (displacement.X > 0 ? 1 : (displacement.X < 0 ? -1 : 0)),
                    (displacement.Y > 0 ? 1 : (displacement.Y < 0 ? -1 : 0)),
                    (displacement.Z > 0 ? 1 : (displacement.Z < 0 ? -1 : 0))
                };

                // t intersection values found in each dimension
                float[] tDim = { 5, 5, 5 };

                char[] charDim =
                {
                    'x',
                    'y',
                    'z'
                };

                for (int dim = 0; dim < 3; ++dim)
                {
                    if (signDim[dim] != 0)
                    {
                        float t;

                        /*
                        This is a rough description of the t-value finder
                        Assuming the player is moving in the +x direction,
                        The player's maxX (right side) will hit the object's minX (left side)
                        If they intersect, then voxelMinX (left side of object) should be inbetween the player's initial right and ending right

                        playerMaxX     voxelMinX  desiredMaxX
                                |             |          |
                        x-----------------------------

                            */
                        if (signDim[dim] == 1)
                        {
                            // player's max.X will possibly collide with object's min.X
                            t = (aabb.min[dim] - playerAABB.max[dim]) / (desiredPlayerAABB.max[dim] - playerAABB.max[dim]);
                        }
                        else
                        {
                            t = (playerAABB.min[dim] - aabb.max[dim]) / (playerAABB.min[dim] - desiredPlayerAABB.min[dim]);
                        }
                        if (t >= 0 && t <= 1)
                        {
                            /*
                            It is possible that there is no actual collision at the found t-value
                            As seen below, if the x-values collide, they still may not be touching
                            y
                            |          ___________
                            |          |         |
                            |          |         |
                            |          L_________|
                            |
                            |
                            |  _________
                            |  |       |
                            |  |       |
                            |  |       |
                            |  L_______|
                            |
                            -------------------------x


                            So we need to see if they overlap just by looking at the y direction:

                            y
                            |
                            |
                            []
                            [] (right square)
                            []
                            []
                            |
                            |
                            []
                            []
                            [] (left square)
                            []
                            []
                            |

                            They don't so we don't count this as an intersection
                            For 3d, we imagine the squares as cubes and line segments as squares (in the y-z plane, using x as an example)
                                */
                            Vector3 collisionPlayerPosition = playerAABB.GetCenter() + displacement * t;
                            AABB possibleCollisionAABB = AABB.FromPositionAndScale(collisionPlayerPosition, player.GetSize());
                            if (possibleCollisionAABB.DoSquaresIntersect(aabb, charDim[dim]))
                            {
                                tDim[dim] = t;
                            }
                        }
                    }
                }

                // Actually move
                float closestT = tDim.Min();
                if (closestT < 4) // They default to 5. Checks if a collision actually happened
                {
                    Vector3 newDisplacement = displacement; // New desired direction from new position
                    Vector3 safeToMove = new Vector3(); // The amount to move toward the collided AABB
                    Vector3 pushBack = new Vector3();

                    for (int dim = 0; dim < 3; ++dim)
                    {
                        if (closestT == tDim[dim])
                        {
                            newDisplacement[dim] = 0;
                            safeToMove[dim] = displacement[dim] * closestT;
                            pushBack[dim] = 0.0001f * -signDim[dim];
                            break;
                        }
                    }

                    player.MoveBy(safeToMove + pushBack);
                    return MoveInScene(player, scene, newDisplacement, depth + 1);
                }
            }
            return player.GetPosition() + displacement;
        }

        public static Voxel? RayTraceVoxel(Vector3 rayOrigin, Vector3 rayDirection, float length, Scene scene)
        {
            if (rayDirection == Vector3.Zero)
                return null;

            Vector3 rayEndPosition = rayOrigin + rayDirection * length;
            rayDirection.Normalize();
            AABB rayAABB = new AABB(rayOrigin, rayOrigin + rayDirection * length);

            List<(Voxel, AABB)> possibleVoxels = new List<(Voxel, AABB)>();

            foreach (Voxel voxel in scene.GetVoxels())
            {
                AABB voxelAABB = AABB.FromPositionAndScale(voxel.GetPosition(), new Vector3(1.0f));
                if (rayAABB.Intersects(voxelAABB))
                {
                    possibleVoxels.Add((voxel, voxelAABB));
                }
            }

            float overallClosestT = 5; // t goes between 0 and 1, and represents 0 to length
            Voxel? closestVoxel = null;
            foreach ((Voxel, AABB) voxelAABBPair in possibleVoxels)
            {
                Voxel voxel = voxelAABBPair.Item1;
                AABB voxelAABB = voxelAABBPair.Item2;

                float[] signDim =
                {
                    (rayDirection.X > 0 ? 1 : (rayDirection.X < 0 ? -1 : 0)),
                    (rayDirection.Y > 0 ? 1 : (rayDirection.Y < 0 ? -1 : 0)),
                    (rayDirection.Z > 0 ? 1 : (rayDirection.Z < 0 ? -1 : 0))
                };

                // t intersection values found in each dimension
                float[] tDim = { 5, 5, 5 };

                char[] charDim =
                {
                    'x',
                    'y',
                    'z'
                };

                for (int dim = 0; dim < 3; ++dim)
                {
                    if (signDim[dim] != 0)
                    {
                        float t;
                        if (signDim[dim] == 1)
                        {
                            t = (voxelAABB.min[dim] - rayOrigin[dim]) / (rayEndPosition[dim] - rayOrigin[dim]);
                        }
                        else
                        {
                            t = (rayOrigin[dim] - voxelAABB.max[dim]) / (rayOrigin[dim] - rayEndPosition[dim]);
                        }
                        if (t >= 0 && t <= 1)
                        {
                            Vector3 intersectionPosition = rayOrigin + rayDirection * length * t;
                            AABB possibleCollisionAABB = AABB.FromPositionAndScale(intersectionPosition, Vector3.Zero);
                            if (possibleCollisionAABB.DoSquaresIntersect(voxelAABB, charDim[dim]))
                            {
                                tDim[dim] = t;
                            }
                        }
                    }
                }

                float voxelClosestT = tDim.Min();
                if (voxelClosestT < overallClosestT) // They default to 5. Checks if a collision actually happened
                {
                    overallClosestT = voxelClosestT;
                    closestVoxel = voxel;
                }
            }
            return closestVoxel;
        }
    }
}
