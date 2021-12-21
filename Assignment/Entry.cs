using Microsoft.VisualBasic.CompilerServices;

namespace Assignment
{
    /// <summary>
    /// Available operations.
    /// </summary>
    public enum Operation
    {
        Add,
        Subtract,
        Multiply
    }
    
    /// <summary>
    /// Calculator input entry.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Register name.
        /// </summary>
        public string Register { get; set; }

        /// <summary>
        /// Operation that should be performed. 
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Value in the input entry.
        /// </summary>
        public string Value { get; set; }
    }
}