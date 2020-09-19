using GlobalPayments.Api.Network.Abstractions;
using System;
using System.IO;

namespace GlobalPayments.Api.Tests {
    public class StanGenerator : IStanProvider {
        private readonly Object objectLock = new Object();

        private static StanGenerator _instance;
        public static StanGenerator GetInstance() {
        if (_instance == null) {
            _instance = new StanGenerator();
        }
        return _instance;
    }

        private int currNumber = 0;
        private string fileName = "C:\\temp\\stan.dat";

        private StanGenerator() {
        lock(objectLock) {
            StreamReader br = null;
            try {
                br = new StreamReader(File.OpenRead(fileName));
                string savedValue = br.ReadLine();
                if (savedValue != null) {
                    currNumber = int.Parse(savedValue);
                }
                else SaveCurrNumber();
            }
            catch (IOException) {
                SaveCurrNumber();
            }
            finally {
                if (br != null) {
                    try {
                        br.Close();
                    }
                    catch (IOException) { /* NOM NOM */ }
                }
            }
        }
    }

        public int GenerateStan() {
        lock(objectLock) {
            if (currNumber == 9999) {
                currNumber = 1;
            }
            else currNumber += 1;
            SaveCurrNumber();
        }
        return currNumber;
    }

        public void Reset() {
            lock (objectLock) {
            currNumber = 0;
            SaveCurrNumber();
        }
    }

        private void SaveCurrNumber() {
        StreamWriter bw = null;
        try {
            bw = new StreamWriter(File.OpenWrite(fileName));
            bw.Write(currNumber + "");
        }
        catch (IOException) {
            /* NOM NOM */
        }
        finally {
            if (bw != null) {
                try {
                    bw.Flush();
                    bw.Close();
                }
                catch (IOException) { /* NOM NOM */ }
            }
        }
    }
    }

}
