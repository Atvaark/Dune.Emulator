namespace Dune.Packets
{
    public enum PacketType : uint
    {
        OnlineCheckRequest                        = 0x10010100,
        OnlineCheckResponse                       = 0x10010200,

        DisconnectionRequest                    = 0x10100100,
        DisconnectionResponse                   = 0x10100200,
        DisconnectionNotification               = 0x10101000,

        ReconnectionRequest                     = 0x10111000,

        FastDataRequest                         = 0x10200100,
        FastDataResponse                        = 0x10200200,

        ConnectionSummaryNotification           = 0x10211000,

        AuthenticationInformationRequestHeader  = 0x11010100,
        AuthenticationInformationResponseHeader = 0x11010200,
        AuthenticationInformationRequestData    = 0x11020100,
        AuthenticationInformationResponseData   = 0x11020200,
        AuthenticationInformationRequestFooter  = 0x11030100,
        AuthenticationInformationResponseFooter = 0x11030200,

        TusCommonAreaAcquisitionRequest         = 0x12010100,
        TusCommonAreaAcquisitionResponse        = 0x12010200,
        ////TusCommonAreaAcquisitionResponseFail    = 0x120102ff,

        TusCommonAreaSettingsRequest            = 0x12020100,
        TusCommonAreaSettingsResponse           = 0x12020200,

        TusCommonAreaAddRequest                 = 0x12030100,
        TusCommonAreaAddResponse                = 0x12030200,

        TusUserAreaWriteRequestHeader           = 0x12040100,
        TusUserAreaWriteResponseHeader          = 0x12040200,
        TusUserAreaWriteRequestData             = 0x12050100,
        TusUserAreaWriteResponseData            = 0x12050200,
        TusUserAreaWriteRequestFooter           = 0x12060100,
        TusUserAreaWriteResponseFooter          = 0x12060200,

        TusUserAreaReadRequestHeader            = 0x12070100,
        TusUserAreaReadResponseHeader           = 0x12070200,
        TusUserAreaReadRequestData              = 0x12080100,
        TusUserAreaReadResponseData             = 0x12080200,
        TusUserAreaReadRequestFooter            = 0x12090100,
        TusUserAreaReadResponseFooter           = 0x12090200
    }
}