using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Class to represent a line from an individual to their parent family or individual on a tree.</summary>
    public class TreeFamilyLine
    {
        #region Member Variables

        /// <summary>The position of the family / parent that this line connects to.</summary>
        private int _parentJoinPosition;

        /// <summary>The height of the connection bar for this family.</summary>
        private int _lineHeight;

        /// <summary>The positions of the children that connect to this family.</summary>
        private readonly List<int> _children;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the TreeFamilyLine class.
        /// </summary>
        /// <param name="parentJoinPosition">Specifies the position of the join for the family / individual.</param>
        public TreeFamilyLine(int parentJoinPosition)
        {
            _parentJoinPosition = parentJoinPosition;
            _lineHeight = 0;
            _children = new List<int>();
        }

        #endregion

        #region Properties.

        /// <summary>The position of the family / individual that this line connects to.</summary>
        public int parentJoinPosition
        {
            get { return _parentJoinPosition; }
        }



        /// <summary>The height of the connection bar for this family.</summary>
        public int lineHeight
        {
            get { return _lineHeight; }
            set { _lineHeight = value; }
        }



        /// <summary>The positions of the children that connect to this family.</summary>
        public List<int> children
        {
            get { return _children; }
        }



        /// <summary>The left most position in this family.</summary>
        public int leftPos
        {
            get
            {
                if (_children.Count == 0)
                {
                    return _parentJoinPosition;
                }
                int leftPos = _parentJoinPosition;
                foreach (int childPos in _children)
                {
                    if (childPos < leftPos)
                    {
                        leftPos = childPos;
                    }
                }
                return leftPos;
            }
        }



        /// <summary>The right most position in this family.</summary>
        public int rightPos
        {
            get
            {
                if (_children.Count == 0)
                {
                    return _parentJoinPosition;
                }
                int rightPos = _parentJoinPosition;
                foreach (int childPos in _children)
                {
                    if (childPos > rightPos)
                    {
                        rightPos = childPos;
                    }
                }
                return rightPos;
            }
        }

        #endregion
    }
}
