using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GlobalPayments.Api.Terminals.INGENICO.Interfaces {
    public class SerialPort {

        #region P/Invokes
        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern bool listPort(
            [MarshalAs(UnmanagedType.BStr)] ref string port,
            [MarshalAs(UnmanagedType.BStr)] ref string desc,
            [MarshalAs(UnmanagedType.BStr)] ref string hwId,
            int index);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static extern string getPort();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getBaudrate();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getByteSize();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getParityBit();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getStopBit();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getFlowControl();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern long getTimeoutRIT();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern long getTimeoutRTTC();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern long getTimeoutRTTM();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern long getTimeoutWTTC();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern long getTimeoutWTTM();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern bool isOpen();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setPort([MarshalAs(UnmanagedType.BStr)] string port);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setBaudrate(int baudrate);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setByteSize(int byteSize);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setParityBit(int parityBit);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setStopBit(int stopBit);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setflowControl(int flowControl);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setTimeoutRIT(long readIntervalTimeout);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setTimeoutRTTC(long readTotalTimeoutConstant);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setTimeoutRTTM(long readTotalTimeoutMultiplier);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setTimeoutWTTC(long writeTotalTimeoutConstant);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern void setTimeoutWTTM(long writeTotalTimeoutMultiplier);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern bool waitReadable(int timeout);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern bool waitByteTimes(int count);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern bool waitForChange();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int initialize();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int open();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int close();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int available(ref long bytesAvaialble);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int read([MarshalAs(UnmanagedType.BStr)] ref string buffer, long size);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int write(string data, long size);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int flush();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int flushInput();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int flushOutput();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int sendBreak(int duration);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int setEscapeCommFunction(int escapeFunction);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int getCommModemStatus(ref int modemStatus);

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int readLock();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int readUnlock();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int writeLock();

        [DllImport("ING-CSRP-SerialLibrary.dll")]
        public static extern int writeUnlock();
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct PortInfo {
            [MarshalAs(UnmanagedType.BStr)]
            public string portName;

            [MarshalAs(UnmanagedType.BStr)]
            public string description;

            [MarshalAs(UnmanagedType.BStr)]
            public string hardwareId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Timeout {
            public long readIntervalTimeout;
            public long readTotalTimeoutConstant;
            public long readTotalTimeoutMultiplier;
            public long writeTotalTimeoutConstant;
            public long writeTotalTimeoutMultiplier;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SerialParameters {
            [MarshalAs(UnmanagedType.BStr)]
            public string portName;
            public int baudrate;
            public Timeout timeout;
            public int byteSize;
            public int parityBit;
            public int stopBit;
            public int flowControl;
        }
        #endregion

        #region Internal Methods
        public static List<PortInfo> getPortInfoList() {
            List<PortInfo> portInfoList = new List<PortInfo>();
            PortInfo portInfo = new PortInfo();

            string port = "";
            string desc = "";
            string hwId = "";
            int index = 0;

            while (listPort(ref port, ref desc, ref hwId, index)) {
                index++;

                portInfo.portName = port;
                portInfo.description = desc;
                portInfo.hardwareId = hwId;

                portInfoList.Add(portInfo);
            }

            return portInfoList;
        }

        public static SerialParameters getSerialParameters() {
            SerialParameters sp = new SerialParameters();

            sp.portName = getPort();
            sp.baudrate = getBaudrate();
            sp.timeout = getTimeout();
            sp.byteSize = getByteSize();
            sp.parityBit = getParityBit();
            sp.stopBit = getStopBit();
            sp.flowControl = getFlowControl();

            return sp;
        }

        public static void setSerialParameters(SerialParameters sp) {
            setPort(sp.portName);
            setBaudrate(sp.baudrate);
            setTimeout(sp.timeout);
            setByteSize(sp.byteSize);
            setParityBit(sp.parityBit);
            setStopBit(sp.stopBit);
            setflowControl(sp.flowControl);
        }

        public static Timeout getTimeout() {
            Timeout timeout = new Timeout();

            timeout.readIntervalTimeout = getTimeoutRIT();
            timeout.readTotalTimeoutConstant = getTimeoutRTTC();
            timeout.readTotalTimeoutMultiplier = getTimeoutRTTM();
            timeout.writeTotalTimeoutConstant = getTimeoutWTTC();
            timeout.writeTotalTimeoutMultiplier = getTimeoutWTTM();

            return timeout;
        }

        public static void setTimeout(Timeout timeout) {
            setTimeoutRIT(timeout.readIntervalTimeout);
            setTimeoutRTTC(timeout.readTotalTimeoutConstant);
            setTimeoutRTTM(timeout.readTotalTimeoutMultiplier);
            setTimeoutWTTC(timeout.writeTotalTimeoutConstant);
            setTimeoutWTTM(timeout.writeTotalTimeoutMultiplier);
        }
        #endregion
    }
}