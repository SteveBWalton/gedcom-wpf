﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom
{
    /// <summary>Class to represent an individual in a gedcom.</summary>
    public class Individual : TopLevel
    {
        #region Member Variables

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public Individual()
        {
        }

        /// <summary>Create an individual from the specified tag.</summary>
        /// <param name="tag">Specifies the tag to build the individual from.</param>
        public Individual(Tag tag) : base(tag)
        {
        }

        #endregion

        #region Properties

        public string fullName
        {
            get
            {
                Tag tagName = _tag.children.findOne("NAME");
                if (tagName == null)
                {
                    return "Error";
                }

                // This is not really correct.
                return tagName.value;
            }
        }

        #endregion
    }
}
