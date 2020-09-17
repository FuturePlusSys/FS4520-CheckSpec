using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS4500_VTests_ML_Functions
{
    public class StateFieldsRdr
    {
        #region Members

        private class FieldInfo
        {
            public string FieldName { get; set; } = "";
            public int FieldOffset { get; set; } = -1;
            public int FieldWidth { get; set; } = -1;

            public FieldInfo(string name, int offset, int width)
            {
                FieldName = name;
                FieldOffset = offset;
                FieldWidth = width;
            }
        }

        private List<FieldInfo> m_fieldInfo = new List<FieldInfo>();
        #endregion // Members

        #region Constructor(s)

        /// <summary>
        /// Default Constructor
        /// </summary>
        public StateFieldsRdr()
        {
        }
        #endregion // Constructor(s)

        #region Private Methods

        /// <summary>
        /// Get the field location in terms of byteID and the MSBit of the field within the byte. 
        /// </summary>
        /// <param name="filterBitID"></param>
        /// <param name="byteID"></param>
        /// <param name="bitID"></param>
        /// <returns></returns>
        /// Assumptions 
        ///                                                                            byte[0]                byte[1]  ...
        ///     OffSet is the bitID starting from the left side and counting up... 7,6,5,4,3,2,1,0  ||  7,6,5,4,3,12,1,0  || ...
        ///                                                                        | | |                | | |
        ///                                                                        | | |                | | |
        ///                                                                        | | offset:2         | | offset:10
        ///                                                                        | offset:1           | offset:9
        ///                                                                        offset:0             offset:8
        private bool getFieldLocation(int byteCount, int offset, ref int byteID, ref int bitID)
        {
            bool status = true;
            bitID = 0;

            // this value represents the byte in linear order from the beginning of the array (starting with byte 0, 1, etc)
            float byteBitIDs = ((float)(offset) / (float)8);

            // this value represents the byte in the array i.e. byte[0]  byte[1]  byte[2]...
            byteID = (int)byteBitIDs;

            // strip off the integer and keep the remainder 
            float bitValue = (float)byteBitIDs - (int)byteBitIDs;

            // identify the bit assoicated with the remainder
            if (bitValue == 0)
                bitID = 7;
            else if (bitValue == 0.125)
                bitID = 6;
            else if (bitValue == 0.25)
                bitID = 5;
            else if (bitValue == 0.375)
                bitID = 4;
            else if (bitValue == 0.50)
                bitID = 3;
            else if (bitValue == 0.625)
                bitID = 2;
            else if (bitValue == 0.75)
                bitID = 1;
            else if (bitValue == 0.875)
                bitID = 0;
            else
            {
                status = false;
                bitID = -1;
            }

            return status;
        }

        /// <summary>
        /// Extract the number of specified consecutive bits from a given byte array
        /// </summary>
        /// <param name="byteID"></param>
        /// <param name="bitID"></param>
        /// <returns></returns>
        private long getFieldValue(int byteID, int bitID, int width, byte[] data)
        {
            long fldValue = 0x00;
            int bitCount = 0;

            // How this works:
            //  'For' loop accumulates the field in chunks of bytes..
            //   The first byte being extracted lops off the MSBits not contained in the field
            //   Thus the bits used in the MSBytes begin with the specified field byte # and Bit #
            //   the loop gets whole bytes for all subsequent bytes being extracted from the data
            //   Once enough bits are accumulated, the loop is exited.  The field value is shifted
            //   to the rights for the number of 'Extra' bits of extracted data.

            // assemble the bytes of data containing the field.
            for (int i = byteID; (width - bitCount) > 0; i++)
            {
                if (i == byteID)
                {
                    // lop off the leading bits...
                    //fldValue = data[i] & (long)(Math.Pow(2, bitID) - 1); //(~((long)(Math.Pow(2, bitID) - 1)));
                    fldValue = data[i] & (uint)(Math.Pow(2, bitID + 1) - 1);
                    bitCount += bitID + 1;
                }
                else
                {
                    fldValue = (fldValue << 8) | data[i];
                    bitCount += 8;
                }
            }

            // lop off the trailing bits that are not part of the field.
            if (bitCount > width)
                fldValue = fldValue >> (bitCount - width);

            return fldValue;
        }


        /// <summary>
        /// Extract a field value at the specified location in the given state data 
        /// </summary>
        /// <param name="fieldOffset"></param>
        /// <param name="FieldWidth"></param>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        private long getLoopFieldData(int fieldOffset, int fieldWidth, byte[] dataBytes)
        {
            long fldValue = -3;
            int byteID = -1;
            int bitID = -1;

            // get the location, in terms of byte ID and Byte bit ID, of the MSBit of the field
            if ( getFieldLocation(dataBytes.Length, fieldOffset, ref byteID, ref bitID) == true)
                fldValue = getFieldValue(byteID, bitID, fieldWidth, dataBytes);

            return fldValue;
        }


        /// <summary>
        /// Get the sub-field data
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int getFieldData(byte[] stateData, int offset, int width)
        {
            int fldValue = -2;

            // this is where we actually extract the data
            fldValue = (int)getLoopFieldData(offset, width, stateData);

            return fldValue;
        }


        /// <summary>
        /// Get the time field data
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private long getTimeFieldData(byte[] stateData, int offset, int width)
        {
            long fldValue = -1;

            // this is where we actually extract the data
            fldValue = getLoopFieldData(offset, width, stateData);

            return fldValue;
        }

        /// <summary>
        /// Get the field offset 
        /// </summary>
        /// <param name="fldName"></param>
        /// <returns></returns>
        private int getFieldParameters(string fldName, ref int offset, ref int width)
        {
            offset = -1;
            width = -1;

            foreach (FieldInfo info in m_fieldInfo)
            {
                if (info.FieldName == fldName)
                {
                    offset = info.FieldOffset;
                    width = info.FieldWidth;
                    break;
                }
            }

            return offset;
        }

        #endregion // Private Methods

        #region Public Methods

        /// <summary>
        /// Initialize the state field meta data
        /// </summary>
        public void Initialize()
        {
            int offset = 0;
            // initialize the fields structure...
            m_fieldInfo.Add(new FieldInfo("Any_Err", offset, 1));
            offset += 1;

            m_fieldInfo.Add(new FieldInfo("Spare", offset, 11));
            offset += 11;

            m_fieldInfo.Add(new FieldInfo("Trigger", offset, 1));
            offset += 1;

            m_fieldInfo.Add(new FieldInfo("Time", offset, 50));
            offset += 50;

            m_fieldInfo.Add(new FieldInfo("Error", offset, 3));
            offset += 3;

            m_fieldInfo.Add(new FieldInfo("Spare", offset, 3));
            offset += 3;

            m_fieldInfo.Add(new FieldInfo("PIXEL_NOT_REC", offset, 1));
            offset += 1;

            m_fieldInfo.Add(new FieldInfo("EventCode", offset, 8));
            offset += 8;

            m_fieldInfo.Add(new FieldInfo("Spare", offset, 6));
            offset += 6;

            m_fieldInfo.Add(new FieldInfo("LOS", offset, 4));
            offset += 4;

            m_fieldInfo.Add(new FieldInfo("Lane0", offset, 10));
            offset += 10;

            m_fieldInfo.Add(new FieldInfo("Lane1", offset, 10));
            offset += 10;

            m_fieldInfo.Add(new FieldInfo("Lane2", offset, 10));
            offset += 10;

            m_fieldInfo.Add(new FieldInfo("Lane3", offset, 10));
            offset += 10;
        }


        /// <summary>
        /// Get the named field's value
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="fldName"></param>
        /// <returns></returns>
        public int GetFieldData(byte[] stateData, string fldName)
        {
            int fldValue = -1;
            int offset = -1;
            int width = -1;

            getFieldParameters(fldName, ref offset, ref width);

            // assumes the field name is NOT Time
            fldValue = getFieldData(stateData, offset, width);

            return fldValue;
        }


        /// <summary>
        /// Get the time field value
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public long GetTime(byte[] stateData)
        {
            long fldValue = -1;
            int offset = -1;
            int width = -1;

            getFieldParameters("Time", ref offset, ref width);

            // assumes the field name is NOT Time
            fldValue = getTimeFieldData(stateData, offset, width);

            return fldValue;
        }


        /// <summary>
        /// Returns an EventCode assoiced with the Byte value.
        /// </summary>
        /// <param name="eventCodeValue"></param>
        /// <returns></returns>
        public string GetEventCodeName_SST(int eventCodeValue)
        {
            string name = "";
            switch (eventCodeValue)
            {
                case 0x88:
                    name = "F0 Pixel";
                    break;
                case 0xC8:
                    name = "F1 Pixel";
                    break;
                case 0x8D:
                    name = "F0 Compressed Pixel";
                    break;
                case 0xCD:
                    name = "F1 Compressed Pixel";
                    break;
                case 0x8E:
                    name = "F0 ECO";
                    break;
                case 0xCE:
                    name = "F1 EOC";
                    break;
                case 0x90:
                    name = "F0 Stuff";
                    break;
                case 0xD0:
                    name = "F1 Stuff";
                    break;

                case 0x28:
                    name = "Hor. CPBS";
                    break;
                case 0x68:
                    name = "Ver. CPBS";
                    break;

                case 0x30:
                    name = "Hor. SR";
                    break;
                case 0x70:
                    name = "Ver. SR";
                    break;

                case 0x0A:
                    name = "Hor. BS";
                    break;
                case 0x4A:
                    name = "Ver. BS";
                    break;

                case 0x0B:
                    name = "Hor. SR";
                    break;
                case 0x4B:
                    name = "Ver. SR";
                    break;

                case 0x15:
                    name = "Hor. BE";
                    break;
                case 0x55:
                    name = "Ver. BE";
                    break;


                case 0x01:
                    name = "TSP1";
                    break;
                case 0x02:
                    name = "TSP2";
                    break;
                case 0x03:
                    name = "TSP3";
                    break;
                case 0x04:
                    name = "TSP4";
                    break;

                case 0x17:
                    name = "Hor. IDLE";
                    break;
                case 0x57:
                    name = "Ver. IDLE";
                    break;

                case 0x09:
                    name = "Hor. VBID";
                    break;
                case 0x49:
                    name = "Ver. VBID";
                    break;


                case 0x0C:
                    name = "Hor. Mvid";
                    break;
                case 0x4C:
                    name = "Ver. Mvid";
                    break;

                case 0x11:
                    name = "Hor. Maud";
                    break;
                case 0x51:
                    name = "Ver. Maud";
                    break;

                case 0x1C:
                    name = "Hor. MSA";
                    break;
                case 0x5C:
                    name = "Ver. MSA";
                    break;

                case 0x1A:
                    name = "Hor. Ordered Set Sleep";
                    break;
                case 0x5A:
                    name = "Ver. Ordered Set Sleep";
                    break;

                case 0x1B:
                    name = "Hor. Ordered Set Standby";
                    break;
                case 0x5B:
                    name = "Ver. Ordered Set Standby";
                    break;

                case 0x20:
                    name = "Hor. Audio Stream";
                    break;
                case 0x60:
                    name = "Ver. Audio Stream";
                    break;

                case 0x24:
                    name = "Hor. Audio TS";
                    break;
                case 0x64:
                    name = "Ver. Audio TS";
                    break;

                case 0x2B:
                    name = "Hor. Audio Copy Mgmt SDP";
                    break;
                case 0x6B:
                    name = "Ver. Audio Copy Mgmt SDP";
                    break;

                case 0x32:
                    name = "Hor. ISRC SDP";
                    break;
                case 0x72:
                    name = "Ver. ISRC SDP";
                    break;

                case 0x12:
                    name = "Hor. VSC SDP";
                    break;
                case 0x52:
                    name = "Ver. VSC SDP";
                    break;

                case 0x3C:
                    name = "Hor. Extension SDP";
                    break;
                case 0x7C:
                    name = "Ver. Extension SDP";
                    break;

                case 0x14:
                    name = "Hor. Info Frame SDP";
                    break;
                case 0x54:
                    name = "Ver. Info Frame SDP";
                    break;

                case 0x29:
                    name = "Hor. Camera";
                    break;
                case 0x69:
                    name = "Ver. Camera";
                    break;

                case 0x21:
                    name = "Hor. PPS";
                    break;
                case 0x61:
                    name = "Ver. PPS";
                    break;

                case 0x22:
                    name = "Hor. VSC EXT VESA";
                    break;
                case 0x62:
                    name = "Ver. VSC EXT VESA";
                    break;

                case 0x25:
                    name = "Hor. VSC EXT CEA";
                    break;
                case 0x65:
                    name = "Ver. VSC EXT CEA";
                    break;

                case 0x34:
                    name = "Hor. FEC PM";
                    break;
                case 0x35:
                    name = "Ver. FEC PH";
                    break;

                case 0x36:
                    name = "Hor. FEC Enable";
                    break;
                case 0x37:
                    name = "Ver. FEC Disable";
                    break;

                case 0x00:
                    name = "Unknown";
                    break;

                default:
                    name = "Reserved";
                    break;
            }

            return name;
        }
        #endregion // Public Methods
    }
}
