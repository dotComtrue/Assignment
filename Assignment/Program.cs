using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment
{   
    class Program
    {
        public static List<Entry> Entries { get; set; }
        
        /// <summary>
        /// Simple calculator program that could add, subtract and multiply values in a set of registrers.
        /// </summary>
        /// <param name="args">File name on input file.</param>
        static int Main(string[] args)
        {
            Entries = new List<Entry>();

            if (args.Any())
            {
                var filePath = args[0];
                ReadFromFile(filePath);
            }
            else
            {
                ReadManualInput();
            }
            return 0;
        }

        /// <summary>
        /// Perform evaluation on input from command line. 
        /// </summary>
        private static void ReadManualInput()
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("Welcome to a simple cmd calculator!");
            Console.WriteLine("You can use operators: add, subtract and multiply.");
            Console.WriteLine("");
            Console.WriteLine("Syntax: <register> <operation> <value>");
            Console.WriteLine("E.g: 'a add 2'");
            Console.WriteLine("To evaluate the result type 'print'.");
            Console.WriteLine("Quit application with 'quit'.");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("");

            var row = string.Empty;

            while (!row.ToLowerInvariant().Equals("quit"))
            {
                row = Console.ReadLine();

                HandleLine(row);
            }
        }

        /// <summary>
        /// Perform evaluation on input from file. 
        /// </summary>
        /// <param name="filePath"></param>
        private static void ReadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    HandleLine(line);
                }
            }
        }

        /// <summary>
        /// Handle a line. 
        /// </summary>
        /// <param name="row"></param>
        private static void HandleLine(string line)
        {
            if (line != null)
            {
                //separate the input parameters 
                var input = line.Trim().Split();

                switch (input.Length)
                {
                    case 0:
                        Console.WriteLine("You must enter an input.");
                        break;

                    case 1:
                        //only 'quit' command consists of one parameter
                        if (input[0].ToLowerInvariant().Trim().Equals("quit"))
                        {
                            {
                                Environment.Exit(0);
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incorrect input.");
                            break;
                        }

                    case 2:
                        //only 'print' command consists of two parameters
                        EvaluatePrintCommand(input);
                        break;

                    case 3:
                        //only an operation entry should consist of three parameters
                        var entry = GetOperationEntry(input);
                        if (entry != null)
                        {
                            Entries.Add(entry);
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            }
        }

        /// <summary>
        /// Get an operation entry object.
        /// </summary>
        /// <param name="input">The operation entry input.</param>
        /// <returns>An Entry object if possible. Otherwise null.</returns>
        private static Entry GetOperationEntry(string[] input)
        {
            var register = input[0].Trim();
            var operation = input[1].Trim();
            var value = input[2].Trim();
            
            if (!register.All(c => char.IsLetterOrDigit(c)))
            {
                Console.WriteLine("Register name must be alphanumeric.");
                return null;
            }

            if (!value.All(c => char.IsLetterOrDigit(c)) && Int32.TryParse(value, out int result))
            {
                Console.WriteLine("Value must either be a alphanumeric register name or a digital input value.");
                return null;
            }

            //check for circular references
            var entriesWithValueAsRegister = Entries.Where(e => e.Register.Equals(value.ToLowerInvariant()));
            if (entriesWithValueAsRegister.Any(c => c.Value.ToLowerInvariant().Equals(register.ToLowerInvariant())))
            {
                Console.WriteLine("Can not create circular reference.");
                return null;
            }

            Operation operationEnum = GetOperationEnum(operation) ?? new Operation();
            
            return new Entry()
            {
                Register = register,
                Operation = operationEnum,
                Value = value
            };
        }

        /// <summary>
        /// Get corresponding Operation enum.
        /// </summary>
        /// <param name="operation">Operation value as string.</param>
        /// <returns>Operation enum.</returns>
        private static Operation? GetOperationEnum(string operation)
        {
            Operation operationEnum;
            if (operation.ToLowerInvariant().Equals(nameof(Operation.Add).ToLowerInvariant()))
            {
                operationEnum = Operation.Add;
            }
            else if (operation.ToLowerInvariant().Equals(nameof(Operation.Multiply).ToLowerInvariant()))
            {
                operationEnum = Operation.Multiply;
            }
            else if (operation.ToLowerInvariant().Equals(nameof(Operation.Subtract).ToLowerInvariant()))
            {
                operationEnum = Operation.Subtract;
            }
            else
            {
                Console.WriteLine($"{operation} is not a valid operation.");
                return null;
            }

            return operationEnum;
        }
        
        /// <summary>
        /// Evaluate and print the result to command line. 
        /// </summary>
        /// <param name="input">Input params.</param>
        private static void EvaluatePrintCommand(string[] input)
        {
            if (!input[0].ToLowerInvariant().Equals("print"))
            {
                Console.WriteLine("Invalid input.");
            }

            string registry = input[1];
            if (!Entries.Any(entry => entry.Register.ToLowerInvariant().Equals(registry.ToLowerInvariant())))
            {
                Console.WriteLine($"There is no registry with name {registry}.");
            }
            
            var result = GetResult(registry);
            if (result == null)
            {
                Console.WriteLine($"Could not calculate result.");
            }
            Console.WriteLine(result.ToString());
        }

        /// <summary>
        /// Calculate the result for the register. 
        /// </summary>
        /// <param name="register">The register.</param>
        /// <returns>The calculated result for the register.</returns>
        private static int? GetResult(string register)
        {
            Entries.ForEach(e => e.Register.ToLowerInvariant().Equals(register.ToLowerInvariant()));

            var result = 0;
            var registerEntries = Entries.Where(entry => entry.Register.ToLowerInvariant().Equals(register.ToLowerInvariant()));
            foreach (var entry in registerEntries)
            {
                var value = 0;
                if (!Int32.TryParse(entry.Value, out value))
                {
                    //Check if it is a registry variable
                    if (!Entries.Any(e => e.Register.ToLowerInvariant().Equals(entry.Value.ToLowerInvariant())))
                    {
                        Console.WriteLine($"There is no registry with name {entry.Value}. Can not calculate. Please give {entry.Value} a value and try again.");
                        return null;
                    }
            
                    //The value is a register; get its result
                    var subResult = GetResult(entry.Value);
                    if (subResult == null)
                    {
                        return null;
                    }
                    value = (int)subResult;
                    
                }
                
                //Do calculation
                switch (entry.Operation)
                {
                    case Operation.Add:
                        result += value;
                        break;
                    case Operation.Subtract:
                        result -= value;
                        break;
                    case Operation.Multiply:
                        result *= value;
                        break;
                    default:
                        return null;
                }
            }
            return result;
        }
    }
}