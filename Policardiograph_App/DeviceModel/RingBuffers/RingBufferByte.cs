using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.DeviceModel.RingBuffers
{
    public class RingBufferByte
    {
         #region Variable Declaration

        private byte[] buf;
        public int wptr;
        public int rptr;
        private int size;
        private int mask;

        #endregion

        #region Constructor

        /// <summary>
        /// RinBuffer constructor
        /// </summary>
        /// <param name="sz2"></param>
        public RingBufferByte(int sz2)
        {
            size = nblock2(sz2);
            buf = new byte[size];
            mask = size - 1;
            wptr = rptr = 0;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the power of 2 that is equal/larger than n
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int npoof2(int n)
        {
            int i = 0;
            --n;
            while (n > 0)
            {
                n >>= 1;
                i++;
            }
            return i;
        }

        /// <summary>
        /// Get the next power of 2 larger/equal to n
        /// </summary>
        /// <param name="n"></param>
        /// <returns>Returns the next power of 2 larger/equal to n</returns>
        public int nblock2(int n)
        {
            return 1 << npoof2(n);
        }

        /// <summary>
        /// Get the number of elements available to be read from the ringbuffer.
        /// </summary>
        /// <returns>Return the number of elements available to be read.</returns>
        public int ReadSpace()
        {
            int w = wptr, r = rptr;
            if (w > r) return w - r;
            else return (size - r + w) & mask;
        }

        /// <summary>
        /// Get the number of elements that will fit into the ringbuffer.
        /// </summary>
        /// <returns>The amount of space available to be written.</returns>
        public int WriteSpace()
        {
            int w = wptr, r = rptr;
            if (w > r) return ((size - w + r) & mask) - 1;
            else if (w < r) return r - w - 1;
            else return size - 1;
        }

        /// <summary>
        /// Reads data out of the ringbuffer into the dest array.
        /// </summary>
        /// <param name="dest">Array to read data into.</param>
        /// <param name="cnt">Requested number of elements to read</param>
        /// <returns>Actual number of elements read</returns>
        public int Read(byte[] dest, int cnt)
        {
            int free_cnt = ReadSpace();
            if (free_cnt == 0) return 0;

            int to_read = cnt > free_cnt ? free_cnt : cnt;
            int cnt2 = rptr + to_read;
            int n1 = 0, n2 = 0;

            if (cnt2 > size)
            {
                n1 = size - rptr;
                n2 = cnt2 & mask;
            }
            else
            {
                n1 = to_read;
                n2 = 0;
            }

            Array.Copy(buf, rptr, dest, 0, n1);
            rptr = (rptr + n1) & mask;

            if (n2 != 0)
            {
                Array.Copy(buf, rptr, dest, n1, n2);
                rptr = (rptr + n2) & mask;
            }
            return to_read;
        }

        /// <summary>
        /// Writes from the src array into the ringbuffer.
        /// </summary>
        /// <param name="src">The souce array to be written to the ringbuffer.</param>
        /// <param name="cnt">The requested number of elements to write.</param>
        /// <returns>The actual number of elements written.</returns>
        public int Write(byte[] src, int cnt)
        {
            int free_cnt = WriteSpace();
            if (free_cnt == 0) return 0;

            int to_write = cnt > free_cnt ? free_cnt : cnt;
            int cnt2 = wptr + to_write;
            int n1 = 0, n2 = 0;

            if (cnt2 > size)
            {
                n1 = size - wptr;
                n2 = cnt2 & mask;
            }
            else
            {
                n1 = to_write;
                n2 = 0;
            }

            Array.Copy(src, 0, buf, wptr, n1);
            wptr = (wptr + n1) & mask;

            if (n2 != 0)
            {
                Array.Copy(src, n1, buf, wptr, n2);
                wptr = (wptr + n2) & mask;
            }
            return to_write;
        }

        /// <summary>
        /// Resets the ringbuffer pointers (will be empty afterwards).
        /// </summary>
        public void Reset()
        {
            rptr = 0;
            wptr = 0;
        }

        /// <summary>
        /// Zero the data in the buffer.
        /// </summary>
        /// <param name="nfloats">Number of elements to zero.</param>
        public void Clear(int nbytes)
        {
            byte[] zero = new byte[nbytes];
            Array.Clear(zero, 0, nbytes);
            Write(zero, nbytes);
        }

        /// <summary>
        /// Reset the pointers and zero the actual data.
        /// </summary>
        /// <param name="nfloats">Number of elements to zero.</param>
        public void Restart(int nbytes)
        {
            Clear(nbytes);
            Reset();
        }

        #endregion
    }
}
