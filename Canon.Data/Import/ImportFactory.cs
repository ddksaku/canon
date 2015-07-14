using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memos.Import;
using Canon.Data.Business;
using Canon.Data.Enums;

namespace Canon.Data.Import
{
    public class ImportFactory
    {
        public static CommonParser<CanonChannelMonitor> GetParser(ChannelTypeEnum type,
                                                                   int channelId,
                                                                   string url,
                                                                   string uploadDir,
                                                                   string additional)
        {
            switch (type)
            {
                case ChannelTypeEnum.XML:
                    CanonChannelImportXml<CanonChannelMonitor> parser = new CanonChannelImportXml<CanonChannelMonitor>(channelId,
                                                                          url,
                                                                          uploadDir);
                    parser.AdditionalCommand = additional;
                    return parser;
                case ChannelTypeEnum.CSV:
                    throw new NotSupportedException("Requested type of parser is not supported. Please see Enums table is DB.");
                default:
                    throw new NotSupportedException("Requested type of parser is not supported. Please see Enums table is DB.");
            }
        }
    }
}
