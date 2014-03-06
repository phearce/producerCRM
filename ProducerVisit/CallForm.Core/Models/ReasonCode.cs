﻿using System;

namespace CallForm.Core.Models
{
    /// <summary>Creates an object representing a "ReasonCode" record.
    /// </summary>
    public class ReasonCode
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>The description of the reason.
        /// </summary>
        public string Name { get; set; }

        /// <summary>The value associated with this <seealso cref="ReasonCode"/>.
        /// </summary>
        public int Code { get; set; }

        /// <summary>The description of this <seealso cref="ReasonCode"/>.
        /// </summary>
        /// <returns>A <seealso cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
