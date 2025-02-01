# AdventCalendar2024

This is a C# console application designed to run solutions for [Advent of Code 2024](https://adventofcode.com/2024). The program allows users to select a specific day's challenge and execute the corresponding solution.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- A terminal or command prompt to run the application

### Running the Program
1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/AdventCalendar2024.git
   cd AdventCalendar2024
   ```

2. Build the project:
   ```sh
   dotnet build
   ```

3. Run the program:
   ```sh
   dotnet run
   ```

4. Enter a day number when prompted. Example:
   ```
   Select day to run (e.g., 1 for Day1):
   ```

   - Enter `1` to run **Day 1's** solution.
   - Enter `102` for **Day 1, Part 2**.
   - Enter `6` for **Day 6**.
   - Enter `601` for **Day 6 Visualization**.

## Project Structure

```
AdventCalendar2024/
│-- Program.cs        # Entry point of the application
│-- DayFactory.cs     # Factory pattern for selecting day solutions
│-- IDay.cs           # Interface for all day solutions
│-- Days/
│   ├── Day1.cs       # Solution for Day 1
│   ├── Day1Part2.cs  # Solution for Day 1 - Part 2
│   ├── Day2.cs       # Solution for Day 2
│   ├── Day2Part2.cs  # Solution for Day 2 - Part 2
│   ├── ...          # Other days and their parts
```

## How It Works

- `Program.cs` prompts the user for a day number.
- `DayFactory.cs` maps day numbers to their respective solution classes.
- Each day's solution implements the `IDay` interface, which requires a `Solve()` method.

Example implementation:

```csharp
public class Day1 : IDay
{
    public string Solve()
    {
        return "Solution for Day 1!";
    }
}
```

## Adding a New Day's Solution

1. Create a new class file (e.g., `Day7.cs` in the `Days/` folder).
2. Implement the `IDay` interface:
   ```csharp
   public class Day7 : IDay
   {
       public string Solve()
       {
           return "Solution for Day 7!";
       }
   }
   ```
3. Register it in `DayFactory.cs`:
   ```csharp
   {7, new Day7()}
   ```

## License
This project is open-source and free to use.

---

Let me know if you want any modifications! 🚀
