using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace PLCApi
{
    public class PLC
    {

        protected TcAdsClient m_adsClient = new TcAdsClient();
        public PLC()
        {
            m_adsClient = new TcAdsClient();
        }
        public TcAdsClient AdsClient
        {
            get
            {
                return m_adsClient;
            }
        }
        
        public void Connect(int port = 851)
        {
            try
            {
               
                m_adsClient.Connect(port);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        void _Connect(string IpAddress, int port)
        {
            try
            {
                m_adsClient = new TcAdsClient();
                m_adsClient.Connect(IpAddress, port);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public void Start()
        {

            try
            {
                m_adsClient.WriteControl(new StateInfo(AdsState.Run, m_adsClient.ReadState().DeviceState));
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public void stop()
        {
            try
            {
                m_adsClient.WriteControl(new StateInfo(AdsState.Stop, m_adsClient.ReadState().DeviceState));
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
