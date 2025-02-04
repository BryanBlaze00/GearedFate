using System;
using UnityEngine;

namespace BTG
{
    public static class VectorHelper2D
    {
        // Define the cardinal and diagonal directions (don't change the order, that will break some method)
        private static Vector2[] _directions = new Vector2[]
        {
            new Vector2(0, 1),  // Up
            new Vector2(1, 1),  // Top-right
            new Vector2(1, 0),  // Right
            new Vector2(1, -1),  // Bottom-right
            new Vector2(0, -1), // Down
            new Vector2(-1, -1),// Bottom-left
            new Vector2(-1, 0), // Left
            new Vector2(-1, 1), // Top-left
        };

        private static Vector2[] _cardinalDirections = new Vector2[]
        {
            new Vector2(0, 1),  // Up
            new Vector2(1, 0),  // Right
            new Vector2(0, -1), // Down
            new Vector2(-1, 0), // Left
        };

        public enum Direction
        {
             Top = 0,
             TopRight =1,
             Right = 2,
             BottomRight = 3,
             Bottom = 4,
             BottomLeft = 5,
             Left = 6,
             TopLeft = 7,
        }

        public static Direction ClosestCardinalOrDiagonal(Vector2 vector)
        {
            // Normalize the input vector
            Vector2 normalizedVector = vector.normalized;

            int chosenDirectionIndex = 0;
            int i = 0;

            // Find the closest direction
            Vector2 closestDirection = _directions[0];
            float maxDot = Vector2.Dot(normalizedVector, closestDirection);

            foreach (Vector2 direction in _directions)
            {
                float dot = Vector2.Dot(normalizedVector, direction.normalized);
                if (dot > maxDot)
                {
                    chosenDirectionIndex = i;
                    maxDot = dot;
                    closestDirection = direction;
                }

                i++;
            }

            return (Direction) chosenDirectionIndex;
        }

        public static Direction ClosestCardinal(Vector2 vector)
        {
            // Normalize the input vector
            Vector2 normalizedVector = vector.normalized;

            int chosenDirectionIndex = 0;
            int i = 0;

            // Find the closest direction
            Vector2 closestDirection = _cardinalDirections[0];
            float maxDot = Vector2.Dot(normalizedVector, closestDirection);

            foreach (Vector2 direction in _cardinalDirections)
            {
                float dot = Vector2.Dot(normalizedVector, direction.normalized);
                if (dot > maxDot)
                {
                    chosenDirectionIndex = i;
                    maxDot = dot;
                    closestDirection = direction;
                }

                i++;
            }

            return (Direction) chosenDirectionIndex;
        }

        public static Vector2 VectorFromDirection(Direction direction)
        {
            return _directions[(int)direction].normalized;
        }

        public static Direction NextClockWiseDirection(Direction direction)
        {
            return (Direction)((int)(direction + 1) % 8);
        }
    }
}
