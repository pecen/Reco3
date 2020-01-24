using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationsLib
{
    public class Axle
    {
        /*
        *  enAxle_1_DrivenNonDriven,
        *  enAxle_1_SingleOrTwin,
        *  enAxle_1_SteeredNonSteered,
        *  enAxle_1_SizeTyre,
        *  enAxle_1_TyreIdentifier,
        */
        protected string m_strDrivenNonDriven;
        public string DrivenNonDriven { get { return m_strDrivenNonDriven; } set { m_strDrivenNonDriven = value; } }

        protected string m_strSingleOrTwin;
        public string SingleOrTwin { get { return m_strSingleOrTwin; } set { m_strSingleOrTwin = value; } }

        protected string m_strSteeredNonSteered;
        public string SteeredNonSteered { get { return m_strSteeredNonSteered; } set { m_strSteeredNonSteered = value; } }

        protected string m_strSizeTyre;
        public string SizeTyre { get { return m_strSizeTyre; } set { m_strSizeTyre = value; } }

        protected string m_strTyreIdentifier;
        public string TyreIdentifier { get { return m_strTyreIdentifier; } set { m_strTyreIdentifier = value; } }

        public bool IsValid { get { return DrivenNonDriven.Length > 1; } }
        public Axle(string strDrivenNonDriven, string strSingleOrTwin, string strSteeredNonSteered, string strSizeTyre, string strTyreIdentifier)
        {
            try
            {
                Initialize(strDrivenNonDriven, strSingleOrTwin, strSteeredNonSteered, strSizeTyre, strTyreIdentifier);
            }
            catch
            {

            }
        }

        public bool Initialize(string strDrivenNonDriven, string strSingleOrTwin, string strSteeredNonSteered, string strSizeTyre, string strTyreIdentifier)
        {
            try
            {
                DrivenNonDriven = strDrivenNonDriven;
                SingleOrTwin = strSingleOrTwin;
                SteeredNonSteered = strSteeredNonSteered;
                SizeTyre = strSizeTyre;
                TyreIdentifier = strTyreIdentifier;

                return true;
            }
            catch
            {

            }
            return false;
        }

    }
}
