using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class PhysicsManager
    {
        public static bool SquareOverlap(Vector2 aMin, Vector2 aMax, Vector2 bMin, Vector2 bMax)
        {
            return !(aMax.X < bMin.X || aMin.X > bMax.X || aMax.Y < bMin.Y || aMin.Y > bMax.Y);
        }

        class AABB
        {
            public Vector3 min;
            public Vector3 max;

            public AABB(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }

            public Vector3 GetCenter()
            {
                return min + (max - min) * 0.5f;
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
                    min: new Vector3(Math.Min(a.min.X, b.min.X), Math.Min(a.min.Y, b.min.Y), Math.Min(a.min.Z, b.min.Z)),
                    max: new Vector3(Math.Max(a.max.X, b.max.X), Math.Max(a.max.Y, b.max.Y), Math.Max(a.max.Z, b.max.Z))
                    );
            }
        }

        public Vector3 MoveInScene(PlayerCamera playerCamera, Scene scene, Vector3 displacement, int depth = 0, int maxDepth = 20)
        {
            if (depth == maxDepth)
                return playerCamera.GetPosition() + displacement;
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
            Vector3 desiredPlayerPosition = playerCamera.GetPosition() + displacement;
            AABB playerAABB = new AABB(playerCamera.GetPosition() - new Vector3(0.5f), playerCamera.GetPosition() + new Vector3(0.5f));
            AABB desiredPlayerAABB = new AABB(desiredPlayerPosition - new Vector3(0.5f), desiredPlayerPosition + new Vector3(0.5f));
            AABB mergedAABB = AABB.Merge(desiredPlayerAABB, playerAABB);

            List<AABB> possibleCollisionAABBs = new List<AABB>();

            foreach (Voxel voxel in scene.GetVoxels())
            {
                AABB voxelAABB = new AABB(voxel.GetPosition() - new Vector3(0.5f), voxel.GetPosition() + new Vector3(0.5f));
                if (mergedAABB.Intersects(voxelAABB))
                {
                    possibleCollisionAABBs.Add(voxelAABB);
                }
            }

            // TODO fence
            foreach (Fence fence in scene.GetFences())
            {
                AABB postAABB = new AABB(fence.GetPosition() - new Vector3(0.1f, 0.5f, 0.1f), fence.GetPosition() + new Vector3(0.1f, 0.5f, 0.1f));
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
                float dist1 = (aabb1.GetCenter() - playerCamera.GetPosition()).Length;
                float dist2 = (aabb2.GetCenter() - playerCamera.GetPosition()).Length;

                return dist1 < dist2 ? -1 : 1;
            }));

            foreach (AABB aabb in possibleCollisionAABBs)
            {
                int xSign = (displacement.X > 0 ? 1 : (displacement.X < 0 ? -1 : 0));
                int ySign = (displacement.Y > 0 ? 1 : (displacement.Y < 0 ? -1 : 0));
                int zSign = (displacement.Z > 0 ? 1 : (displacement.Z < 0 ? -1 : 0));
                float xT = 5;
                float yT = 5;
                float zT = 5;

                if (xSign != 0)
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
                    if (xSign == 1)
                    {
                        // player's max.X will possibly collide with object's min.X
                        t = (aabb.min.X - playerAABB.max.X) / (desiredPlayerAABB.max.X - playerAABB.max.X);
                    }
                    else
                    {
                        t = (playerAABB.min.X - aabb.max.X) / (playerAABB.min.X - desiredPlayerAABB.min.X);
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
                        Vector3 collisionPlayerPosition = playerCamera.GetPosition() + displacement * t;
                        AABB possibleCollisionAABB = new AABB(collisionPlayerPosition - new Vector3(0.5f), collisionPlayerPosition + new Vector3(0.5f));
                        if (possibleCollisionAABB.DoSquaresIntersect(aabb, 'x'))
                        {
                            xT = t;
                        }
                    }
                }

                if (ySign != 0)
                {
                    float t;
                    if (ySign == 1)
                    {
                        t = (aabb.min.Y - playerAABB.max.Y) / (desiredPlayerAABB.max.Y - playerAABB.max.Y);
                    }
                    else
                    {
                        t = (playerAABB.min.Y - aabb.max.Y) / (playerAABB.min.Y - desiredPlayerAABB.min.Y);
                    }
                    if (t >= 0 && t <= 1)
                    {
                        Vector3 collisionPlayerPosition = playerCamera.GetPosition() + displacement * t;
                        AABB possibleCollisionAABB = new AABB(collisionPlayerPosition - new Vector3(0.5f), collisionPlayerPosition + new Vector3(0.5f));
                        if (possibleCollisionAABB.DoSquaresIntersect(aabb, 'y'))
                        {
                            yT = t;
                        }
                    }
                }


                if (zSign != 0)
                {
                    float t;
                    if (zSign == 1)
                    {
                        t = (aabb.min.Z - playerAABB.max.Z) / (desiredPlayerAABB.max.Z - playerAABB.max.Z);
                    }
                    else
                    {
                        t = (playerAABB.min.Z - aabb.max.Z) / (playerAABB.min.Z - desiredPlayerAABB.min.Z);
                    }
                    if (t >= 0 && t <= 1)
                    {
                        Vector3 collisionPlayerPosition = playerCamera.GetPosition() + displacement * t;
                        AABB possibleCollisionAABB = new AABB(collisionPlayerPosition - new Vector3(0.5f), collisionPlayerPosition + new Vector3(0.5f));
                        if (possibleCollisionAABB.DoSquaresIntersect(aabb, 'z'))
                        {
                            zT = t;
                        }
                    }
                }

                if (xT < 4 || yT < 4 || zT < 4) // They default to 5. Checks if a collision actually happened
                {
                    float closestT = MathF.Min(xT, MathF.Min(yT, zT));

                    Vector3 newDisplacement = displacement;
                    Vector3 safeToMove = new Vector3();
                    Vector3 pushBack = new Vector3();

                    if (closestT == xT)
                    {
                        newDisplacement.X = 0;
                        safeToMove.X = displacement.X * closestT;
                        pushBack.X = 0.0001f * -xSign;

                    }
                    else if (closestT == yT)
                    {
                        newDisplacement.Y = 0;
                        safeToMove.Y = displacement.Y * closestT;
                        pushBack.Y = 0.0001f * -ySign;
                    }
                    else
                    {
                        newDisplacement.Z = 0;
                        safeToMove.Z = displacement.Z * closestT;
                        pushBack.Z = 0.0001f * -zSign;
                    }
                    playerCamera.MoveBy(safeToMove + pushBack);
                    return MoveInScene(playerCamera, scene, newDisplacement, depth + 1);
                }
            }
            return playerCamera.GetPosition() + displacement;
        }
    }
}
