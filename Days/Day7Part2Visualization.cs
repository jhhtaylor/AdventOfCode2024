using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day7Part2Visualization : IDay
{
    public string Solve()
    {
        // Path to the input file
        string filePath = "Inputs/Day7Input_Example.txt";

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

            // Display the equation
            Console.WriteLine($"\nEquation: {testValue}: {string.Join(" ", numbers)}");

            // Generate all possible operator combinations
            var operatorCombinations = GenerateOperatorCombinations(numbers.Count - 1);

            // Check if any combination matches the test value
            bool isValid = false;
            foreach (var operators in operatorCombinations)
            {
                // Display the current operator combination
                string expression = BuildExpression(numbers, operators);
                Console.Write($"Testing: {expression} = ");

                // Evaluate the expression
                long result = EvaluateExpression(numbers, operators);
                Console.Write(result);

                // Check if the result matches the test value
                if (result == testValue)
                {
                    Console.WriteLine(" ✅ (Valid)");
                    isValid = true;
                    break; // No need to check other combinations for this equation
                }
                else
                {
                    Console.WriteLine(" ❌ (Invalid)");
                }
            }

            // If the equation is valid, add its test value to the total
            if (isValid)
            {
                totalCalibrationResult += testValue;
                Console.WriteLine($"Equation is valid. Adding {testValue} to the total.");
            }
            else
            {
                Console.WriteLine("Equation is invalid.");
            }
        }

        // Display the total calibration result
        Console.WriteLine($"\nTotal Calibration Result: {totalCalibrationResult}");

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

    /// <summary>
    /// Builds a string representation of the expression.
    /// </summary>
    /// <param name="numbers">The list of numbers.</param>
    /// <param name="operators">The list of operators.</param>
    /// <returns>A string representing the expression.</returns>
    private string BuildExpression(List<long> numbers, List<char> operators)
    {
        var expression = new System.Text.StringBuilder();
        expression.Append(numbers[0]);
        for (int i = 0; i < operators.Count; i++)
        {
            expression.Append($" {operators[i]} {numbers[i + 1]}");
        }
        return expression.ToString();
    }
}