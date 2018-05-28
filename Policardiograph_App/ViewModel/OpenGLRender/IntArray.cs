using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.ViewModel.OpenGLRender
{
    public class IntArray
    {
        public int[] intArray;
        public int size;
        public int index;
        public IntArray(int size)
        {
            this.intArray = new int[size];
            this.size = size;
            this.index = 0;
        }
        public void Clear() {
            for (int i = 0; i < size; i++) {
                intArray[i] = 0;
            }
        }
    }
}