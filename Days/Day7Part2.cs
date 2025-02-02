using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day7Part2 : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day7Input.txt";

        // Check if the input file exists
        if (!File.Exists(filePath))
        {
            return "Error: input.txt not found!";
        }

        // Variable to store the total calibration result
        long totalCalibrationResult = 0;

        // Read the input file line by line
        foreach (string line in File.ReadLines(filePath))
        {
            // Split the line into the test value and the numbers
            var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                continue; // Skip invalid lines
            }

            // Parse the test value
            if (!long.TryParse(parts[0], out long testValue))
            {
                continue; // Skip lines with invalid test values
            }

            // Parse the numbers
            var numbers = parts[1]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(num => long.Parse(num))
                .ToList();

            // Generate all possible operator combinations
            var operatorCombinations = GenerateOperatorCombinations(numbers.Count - 1);

            // Check if any combination matches the test value
            foreach (var operators in operatorCombinations)
            {
                long result = EvaluateExpression(numbers, operators);
                if (result == testValue)
                {
                    // Add the test value to the total calibration result
                    totalCalibrationResult += testValue;
                    break; // No need to check other combinations for this equation
                }
            }
        }

        // Return the total calibration result as a string
        return totalCalibrationResult.ToString();
    }

    /// <summary>
    /// Generates all possible combinations of '+', '*', and '||' operators.
    /// </summary>
    /// <param name="count">The number of operators to generate.</param>
    /// <returns>A list of all possible operator combinations.</returns>
    private List<List<char>> GenerateOperatorCombinations(int count)
    {
        var combinations = new List<List<char>>();
        GenerateOperatorCombinationsHelper(count, new List<char>(), combinations);
        return combinations;
    }

    /// <summary>
    /// Helper method to recursively generate operator combinations.
    /// </summary>
    private void GenerateOperatorCombinationsHelper(int count, List<char> current, List<List<char>> combinations)
    {
        if (count == 0)
        {
            combinations.Add(new List<char>(current));
            return;
        }

        // Try adding '+'
        current.Add('+');
        GenerateOperatorCombinationsHelper(count - 1, current, combinations);
        current.RemoveAt(current.Count - 1);

        // Try adding '*'
        current.Add('*');
        GenerateOperatorCombinationsHelper(count - 1, current, combinations);
        current.RemoveAt(current.Count - 1);

        // Try adding '||'
        current.Add('|');
        GenerateOperatorCombinationsHelper(count - 1, current, combinations);
        current.RemoveAt(current.Count - 1);
    }

    /// <summary>
    /// Evaluates an expression with the given numbers and operators.
    /// </summary>
    /// <param name="numbers">The list of numbers.</param>
    /// <param name="operators">The list of operators.</param>
    /// <returns>The result of the expression.</returns>
    private long EvaluateExpression(List<long> numbers, List<char> operators)
    {
        long result = numbers[0];
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == '+')
            {
                result += numbers[i + 1];
            }
            else if (operators[i] == '*')
            {
                result *= numbers[i + 1];
            }
            else if (operators[i] == '|')
            {
                // Concatenate the numbers
                result = long.Parse(result.ToString() + numbers[i + 1].ToString());
            }
        }
        return result;
    }
}