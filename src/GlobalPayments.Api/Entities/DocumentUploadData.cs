﻿using System;
using System.Collections.ObjectModel;

namespace GlobalPayments.Api.Entities
{
    public class DocumentUploadData {
        /// <summary>
        /// Name the document according to instructions provided to you by ProPay's Risk team
        /// </summary>
        public string DocumentName { get; set; }
        /// <summary>
        /// The transaction number of the chargeback you need to dispute
        /// </summary>
        public string TransactionReference { get; set; }
        /// <summary>
        /// The file format of the Document to be uploaded. This value is set automatically by setting the DocumentPath property
        /// </summary>
        public string DocType { get; private set; }
        /// <summary>
        /// The document data in base64 format. This value is set automatically by setting the DocumentPath property
        /// </summary>
        public string Document { get; private set; }
        /// <summary>
        /// The type of document you've been asked to provide by ProPay's Risk team. Valid values are:
        /// Verification, FraudHolds, Underwriting, RetrievalRequest
        /// </summary>
        public string DocCategory { get; set; }
        public string DocumentPath { 
            set
            {
                var docPath = value;
                if (docPath != null)
                {
                        var docType = docPath.Substring(docPath.LastIndexOf('.') + 1);
                        if (_validDocTypes.Contains(docType))
                        {
                            DocType = docType;
                            Document = Convert.ToBase64String(System.IO.File.ReadAllBytes(docPath));
                        }
                        else
                        {
                            throw new Exception("The document provided is not a valid file type.");
                        }
                    }
                    else
                    {
                        throw new Exception("DocumentPath has not been set");
                    }
            }
        }

        private ReadOnlyCollection<string> _validDocTypes { get; } = new ReadOnlyCollection<string>(
            new string[]
            {
                "tif",
                "tiff",
                "bmp",
                "jpg",
                "jpeg",
                "gif",
                "png",
                "doc",
                "docx"
            }
        );
    }
}
