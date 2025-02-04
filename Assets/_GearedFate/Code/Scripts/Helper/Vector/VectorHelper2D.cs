using System;
using UnityEngine;

namespace BTG
{
    public static class VectorHelper2D
    {
        // Define the cardinal and diagonal directions (don't change the order, that will break some method)
        private static Vector2[] _directions = new Vector2[]
        {
            new(0, 1), // Up
            new(1, 1), // Top-right
            new(1, 0), // Right
            new(1, -1), // Bottom-right
            new(0, -1), // Down
            new(-1, -1), // Bottom-left
            new(-1, 0), // Left
            new(-1, 1) // Top-left
        };

        private static Vector2[] _cardinalDirections = new Vector2[]
        {
            new(0, 1), // Up
            new(1, 0), // Right
            new(0, -1), // Down
            new(-1, 0) // Left
        };

        public enum Direction
        {
            Top = 0,
            TopRight = 1,
            Right = 2,
            BottomRight = 3,
            Bottom = 4,
            BottomLeft = 5,
            Left = 6,
            TopLeft = 7
        }

        public static Direction ClosestCardinalOrDiagonal(Vector2 vector)
        {
            // Normalize the input vector
            var normalizedVector = vector.normalized;

            var chosenDirectionIndex = 0;
            var i = 0;

            // Find the closest direction
            var closestDirection = _directions[0];
            var maxDot = Vector2.Dot(normalizedVector, closestDirection);

            foreach (var direction in _directions)
            {
                var dot = Vector2.Dot(normalizedVector, direction.normalized);
                if (dot > maxDot)
                {
                    chosenDirectionIndex = i;
                    maxDot = dot;
                    closestDirection = direction;
                }

                i++;
            }

            return (Direction)chosenDirectionIndex;
        }

        public static Direction ClosestCardinal(Vector2 vector)
        {
            // Normalize the input vector
            var normalizedVector = vector.normalized;

            var chosenDirectionIndex = 0;
            var i = 0;

            // Find the closest direction
            var closestDirection = _cardinalDirections[0];
            var maxDot = Vector2.Dot(normalizedVector, closestDirection);

            foreach (var direction in _cardinalDirections)
            {
                var dot = Vector2.Dot(normalizedVector, direction.normalized);
                if (dot > maxDot)
                {
                    chosenDirectionIndex = i;
                    maxDot = dot;
                    closestDirection = direction;
                }

                i++;
            }

            return (Direction)chosenDirectionIndex;
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
