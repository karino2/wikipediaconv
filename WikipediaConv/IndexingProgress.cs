using System;
using System.Collections.Generic;
using System.Text;

namespace WikipediaConv
{
    /// <summary>
    /// The state of the indexing process
    /// </summary>
    public struct DecodingProgress
    {
        /// <summary>
        /// The state of the indexing process
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Currently running
            /// </summary>
            Running,
            /// <summary>
            /// Finished
            /// </summary>
            Finished,
            /// <summary>
            /// Failed executing
            /// </summary>
            Failure
        }

        /// <summary>
        /// The state of the indexing process
        /// </summary>
        public State DecodingState;
        /// <summary>
        /// The error message, if any
        /// </summary>
        public string Message;
        /// <summary>
        /// Estimated time of index completion (ETA)
        /// </summary>
        public string ETA;
    }
}
