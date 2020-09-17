using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS4500_VTests_ML_Functions
{
    public class ML_Common_Functions
    {
        #region Members 

        private StateFieldsRdr m_stateFldsRdr = null;
        private List<string> SDPEventCodeNames = null;
        #endregion // Members 

        #region Constructor(s)

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ML_Common_Functions()
        {
            m_stateFldsRdr = new StateFieldsRdr();
            m_stateFldsRdr.Initialize();
            setUpSDPEventCodeNames();
        }

        #endregion // Private Methods

        #region Private Methods


        /// <summary>
        /// Add event code names for SDP packets
        /// </summary>
        private void setUpSDPEventCodeNames()
        {
            SDPEventCodeNames = new List<string>();
            SDPEventCodeNames.Add("Hor. MSA");
            SDPEventCodeNames.Add("Ver. MSA");
            SDPEventCodeNames.Add("Hor. Audio Stream");
            SDPEventCodeNames.Add("Ver. Audio Stream");
            SDPEventCodeNames.Add("Hor. Audio TS");
            SDPEventCodeNames.Add("Ver. Audio TS");
            SDPEventCodeNames.Add("Hor. Audio Copy Mgmt SDP");
            SDPEventCodeNames.Add("Ver. Audio Copy Mgmt SDP");
            SDPEventCodeNames.Add("Hor. ISRC SDP");
            SDPEventCodeNames.Add("Ver. ISRC SDP");
            SDPEventCodeNames.Add("Hor. VSC SDP");
            SDPEventCodeNames.Add("Ver. VSC SDP");
            SDPEventCodeNames.Add("Hor. Extension SDP");
            SDPEventCodeNames.Add("Ver. Extension SDP");
            SDPEventCodeNames.Add("Hor. Info Frame SDP");
            SDPEventCodeNames.Add("Ver. Info Frame SDP");
        }

        #endregion // Private Methods

        #region Public Methods



        /// <summary>
        /// Extract a named integer parameter.
        /// </summary>
        /// <param name="ConfigParameters"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        public int GetConfigParameterValue(List<string> ConfigParameters, string pName)
        {
            int pValue = -2;

            // extract the user selected parameters that we need...
            foreach (string p in ConfigParameters)
            {
                string[] comps = p.Split(new char[] { ':' });
                if (comps.Length == 2)
                {
                    if (comps[0].ToUpper() == pName)
                    {
                        pValue = int.Parse(comps[1]);
                        break;
                    }
                }
                else
                {
                    pValue = -1;
                }
            }

            return pValue;
        }


        /// <summary>
        /// Extract a named integer parameter.
        /// </summary>
        /// <param name="ConfigParameters"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        public string GetConfigParameterValue_String(List<string> ConfigParameters, string pName)
        {
            string pValue = "";

            // extract the user selected parameters that we need...
            foreach (string p in ConfigParameters)
            {
                string[] comps = p.Split(new char[] { ':' });
                if (comps.Length == 2)
                {
                    if (comps[0].ToUpper() == pName)
                    {
                        pValue = comps[1];
                        break;
                    }
                }
                else
                {
                    pValue = "";
                }
            }

            return pValue;
        }


        /// <summary>
        /// Extract a named integer parameter.
        /// </summary>
        /// <param name="ConfigParameters"></param>
        /// <param name="pName"></param>
        /// <returns></returns>
        public bool GetConfigParameterValue_Bool(List<string> ConfigParameters, string pName)
        {
            bool pValue =false;

            // extract the user selected parameters that we need...
            foreach (string p in ConfigParameters)
            {
                string[] comps = p.Split(new char[] { ':' });
                if (comps.Length == 2)
                {
                    if (comps[0].ToUpper() == pName)
                    {
                        pValue = bool.Parse(comps[1]);
                        break;
                    }
                }
                else
                {
                    pValue = false;
                }
            }

            return pValue;
        }

        /// <summary>
        /// Returns a list of integers representing all four lanes in ascending order (lane[0] - lane[3]).
        /// </summary>
        /// <param name="laneID"></param>
        /// <returns></returns>
        public List<int> GetLanesData(byte[] stateData)
        {
            List<int> laneData = new List<int>();

            laneData.Add(m_stateFldsRdr.GetFieldData(stateData, "Lane0"));
            laneData.Add(m_stateFldsRdr.GetFieldData(stateData, "Lane1"));
            laneData.Add(m_stateFldsRdr.GetFieldData(stateData, "Lane2"));
            laneData.Add(m_stateFldsRdr.GetFieldData(stateData, "Lane3"));

            return laneData;
        }

        /// <summary>
        ///  get the index used to index for the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetLaneIndex(string name)
        {
            int index = -1;

            switch (name.ToLower())
            {
                case "lane0":
                    index = 0;
                    break;

                case "lane1":
                    index = 1;
                    break;

                case "lane2":
                    index = 2;
                    break;

                case "lane3":
                    index = 3;
                    break;

                default:
                    break;
            }

            return index;
        }

        /// <summary>
        /// Get the 10 bits of data for a specified lane...
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="laneName"></param>
        /// <returns></returns>
        public int GetLaneData(byte[] stateData, string laneName)
        {
            return m_stateFldsRdr.GetFieldData(stateData, laneName);
        }


        /// <summary>
        /// Get the named sub-field value for the given state data 
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="fldName"></param>
        /// <returns></returns>
        public int GetFieldData(byte[] stateData, string fldName)
        {
            return m_stateFldsRdr.GetFieldData(stateData, fldName);
        }


        /// <summary>
        /// Returns the event code name for the specified 8-Bit integer value.
        /// </summary>
        /// <param name="ECValue"></param>
        /// <returns></returns>
        public string GetEventCodeName(string protocolID, int ECValue)
        {
            string ecName = "";

            if (protocolID == "SST")
                ecName = m_stateFldsRdr.GetEventCodeName_SST(ECValue);

            return ecName;
        }

        public bool IsSDP(string ECName)
        {
            bool status = false;

            if (ECName == "Info Frame SDP")
                status = true;

            return status;
        }

        /// <summary>
        /// Check if the event code name is a SDP
        /// </summary>
        /// <param name="protocolID"></param>
        /// <param name="ECName"></param>
        /// <returns></returns>
        public bool CheckIfEventCodeNameIsSDP(string ECName)
        {
            bool status = false;
            for (int i = 0; i < SDPEventCodeNames.Count; i++)
            {
                if (SDPEventCodeNames[i] == ECName)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }


        /// <summary>
        /// Get the 50 bit time ticks value.
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public long GetTimeTicks(byte[] stateData)
        {
            return m_stateFldsRdr.GetTime(stateData);
        }

        #endregion // Public Methods
    }
}
