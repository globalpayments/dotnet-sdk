using System;
using System.Collections.Generic;
using System.IO;
using GlobalPayments.Api.Entities;

namespace GlobalPayments.Api.Terminals.HPA {
    public class HpaFileUpload {
        private byte[] _buffer;
        private string _filePath;
        internal string _hexData;
        private SendFileType _imageType;
        
        private const string BANNER_FILENAME = "banner.jpg";
        private const string LOGO_FILENAME = "idlelogo.jpg";
        
        public string FileName { get; private set; }
        public int FileSize { get { return _buffer.Length; } }

        public HpaFileUpload(SendFileType imageType, string filePath) {
            _imageType = imageType;
            _filePath = filePath;

            // file name
            FileName = Path.GetFileName(filePath);
            switch (imageType) {
                case SendFileType.Banner: {
                        if (!FileName.Equals(BANNER_FILENAME, StringComparison.OrdinalIgnoreCase)) {
                            throw new ApiException("The filename for banners must be BANNER.JPG.");
                        }
                    } break;
                case SendFileType.Logo: {
                        if (!FileName.Equals(LOGO_FILENAME, StringComparison.OrdinalIgnoreCase)) {
                            throw new ApiException("The filename for banners must be IDLELOGO.JPG.");
                        }
                    } break;
                default: {
                        throw new ApiException("Unknown send file type.");
                    }
            }

            // file size
            _buffer = File.ReadAllBytes(filePath);
            _hexData = BitConverter.ToString(_buffer).Replace("-", "");
        }

        public IEnumerable<string> GetFileParts(int maxDataLength) {
            if (string.IsNullOrEmpty(_hexData)) {
                yield return string.Empty;
            }

            for (var i = 0; i < _hexData.Length; i += maxDataLength) {
                yield return _hexData.Substring(i, Math.Min(maxDataLength, _hexData.Length - i));
            }
        }
    }
}
