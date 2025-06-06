using OpenTK.Mathematics;

namespace Voxel_Project
{
    /// <summary>
    /// Deals with physics in the scene
    /// </summary>
    internal static class PhysicsManager
    {
        public static bool SquareOverlap(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
        {
            return !(aMax.X < bMin.X || aMin.X > bMax.X || aMax.Y < bMin.Y || aMin.Y > bMax.Y);
        }

        /// <summary>
        /// Axis aligned bounding box (non-rotated rentangular prism)
        /// </summary>
        class AABB
        {
            public Vector3 min; // Corner with the minimum x, y, and z values
            public Vector3 max; // Corner with the maximum x, y, and z values

            /// <param name="p1">A corner of the AABB</param>
            /// <param name="p2">A corner of the AABB</param>
            public AABB(Vector3 p1, Vector3 p2)
            {
                this.min = new Vector3(MathF.Min(p1.X, p2.X), MathF.Min(p1.Y, p2.Y), MathF.Min(p1.Z, p2.Z));
                this.max = new Vector3(MathF.Max(p1.X, p2.X), MathF.Max(p1.Y, p2.Y), MathF.Max(p1.Z, p2.Z));
            }

            /// <summary>
            /// Creates an AABB given a center position and the width, height, and depth of the AABB
            /// </summary>
            /// <param name="position">The center position</param>
            /// <param name="scale">The width, height, and depth</param>
            /// <returns></returns>
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
            public AABB NewMoveBy(Vector3 moveBy)
            {
                AABB newAABB = new AABB(min, max);
                newAABB.min += moveBy;
                newAABB.max += moveBy;
                return newAABB;
            }

            /// <summary>
            /// Checks the intersection of 2 AABBs
            /// </summary>
            /// <returns>Whether or not the AABBs overlap</returns>
            public bool Intersects(AABB other)
            {
                return !(this.min.X >= other.max.X || this.max.X <= other.min.X || this.min.Y >= other.max.Y || this.max.Y <= other.min.Y || this.min.Z >= other.max.Z || this.max.Z <= other.min.Z);
            }

            /// <summary>
            /// Checks if the two AABBs overlap when viewed from an axis
            /// </summary>
            /// <param name="dim">Either x, y, or z. The axis to view the AABBs from</param>
            /// <returns>Whether the AABBs overlap, or false, if dim is not one of {'x', 'y', 'z'}</returns>
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

            /// <summary>
            /// Creates the smallest possible AABB that contains all of the points within two AABBs
            /// </summary>
            /// <returns>The merged AABB</returns>
            public static AABB Merge(AABB aabb1, AABB aabb2)
            {
                return new AABB(
                    p1: new Vector3(Math.Min(aabb1.min.X, aabb2.min.X), Math.Min(aabb1.min.Y, aabb2.min.Y), Math.Min(aabb1.min.Z, aabb2.min.Z)),
                    p2: new Vector3(Math.Max(aabb1.max.X, aabb2.max.X), Math.Max(aabb1.max.Y, aabb2.max.Y), Math.Max(aabb1.max.Z, aabb2.max.Z))
                    );
            }
        }

        /// <summary>
        /// Decides where the player will move in a frame
        /// </summary>
        /// <param name="displacement">The movement vector that the player wants to move by</param>
        /// <param name="depth">Recursion depth</param>
        /// <returns>The position that the player should end up at</returns>
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
            Vector3 desiredPlayerCenter = playerCenter + displacement; // Desired means where the player would end up if there were no objects in the scene

            AABB playerAABB = AABB.FromPositionAndScale(playerCenter, player.GetSize());
            AABB desiredPlayerAABB = AABB.FromPositionAndScale(desiredPlayerCenter, player.GetSize());
            AABB mergedAABB = AABB.Merge(desiredPlayerAABB, playerAABB);

            // Filters only the objects that may collide with the player
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

            foreach (AABB currentAABBCheckingAgainst in possibleCollisionAABBs)
            {
                // For each array of 3 items, arr[0] means x, arr[1] means y, and arr[2] means z
                // The motion direction in each dimension
                float[] signDim =
                {
                    (displacement.X > 0 ? 1 : (displacement.X < 0 ? -1 : 0)),
                    (displacement.Y > 0 ? 1 : (displacement.Y < 0 ? -1 : 0)),
                    (displacement.Z > 0 ? 1 : (displacement.Z < 0 ? -1 : 0))
                };

                // t intersection values found in each dimension
                // t is the scaler for the displcement vector, where 1 means all of the displacement, and 0 means none
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
                        If they intersect, then voxelMinX (left side of object) should be inbetween the player's initial right side and desired right side (where they are trying to go)

                        playerMaxX     voxelMinX  desiredMaxX
                                |             |          |
                        -x------------------------------------+x

                            */
                        if (signDim[dim] == 1)
                        {
                            // Player's max.X will possibly collide with object's min.X
                            t = (currentAABBCheckingAgainst.min[dim] - playerAABB.max[dim]) / (desiredPlayerAABB.max[dim] - playerAABB.max[dim]);
                        }
                        else
                        {
                            // Other way around
                            t = (playerAABB.min[dim] - currentAABBCheckingAgainst.max[dim]) / (playerAABB.min[dim] - desiredPlayerAABB.min[dim]);
                        }
                        if (t >= 0 && t <= 1)
                        {
                            /*
                            It is possible that there is no actual collision at the found t-value
                            As seen below (in 2d for simplicity), if the x-values collide, they still may not be touching
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
                            [] (right/higher square)
                            []
                            []
                            |
                            |
                            []
                            []
                            [] (left/lower square)
                            []
                            []
                            |

                            They don't so we don't count this as an intersection
                            For 3d the squares are cubes and the line segments ([]) are squares (in the y-z plane if we're checking the x motion)
                                */
                            Vector3 collisionPlayerPosition = playerAABB.GetCenter() + displacement * t;
                            AABB playerAABBAtPossibleCollision = AABB.FromPositionAndScale(collisionPlayerPosition, player.GetSize());
                            if (playerAABBAtPossibleCollision.DoSquaresIntersect(currentAABBCheckingAgainst, charDim[dim]))
                            {
                                tDim[dim] = t;
                            }
                        }
                    }
                }

                // Actually move
                float closestT = tDim.Min();
                if (closestT < 4) // t-values default to 5. Checks if a collision actually happened
                {
                    Vector3 newDisplacement = displacement; // New desired motion from new position, to be used recursively
                    Vector3 safeToMove = new Vector3(); // The amount to move toward the collided AABB
                    Vector3 pushBack = new Vector3(); // Moves the player back a bit to prevent the player from technically colliding with the object when they move to it

                    for (int dim = 0; dim < 3; ++dim)
                    {
                        if (closestT == tDim[dim])
                        {
                            newDisplacement[dim] = 0; // Flatten velocity against the axis we hit
                            safeToMove[dim] = displacement[dim] * closestT; // Move up to the object
                            pushBack[dim] = 0.0001f * -signDim[dim]; // Go back a little bit
                            break;
                        }
                    }

                    player.MoveBy(safeToMove + pushBack);
                    return MoveInScene(player, scene, newDisplacement, depth + 1);
                }
            }
            return player.GetPosition() + displacement; // If no collisions are found
        }

        /// <summary>
        /// Checks which voxel in the scene is hit by a ray
        /// </summary>
        /// <param name="rayOrigin">The starting position of the ray</param>
        /// <param name="rayDirection">A normalized vector describing the direction of the ray</param>
        /// <param name="length">The max distance the ray can travel</param>
        /// <returns></returns>
        public static Voxel? RayTraceVoxel(Vector3 rayOrigin, Vector3 rayDirection, float length, Scene scene)
        {
            if (rayDirection == Vector3.Zero)
                return null;

            rayDirection.Normalize();
            Vector3 rayEndPosition = rayOrigin + rayDirection * length;
            AABB rayAABB = new AABB(rayOrigin, rayOrigin + rayDirection * length); // AABB that contains the entire path of the ray

            // Filter out voxels that definently won't be hit by using rayAABB
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
                // The algorithm is very similar to the MoveInScene() algorithm, so check that for explanations
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
