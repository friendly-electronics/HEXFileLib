﻿namespace FriendlyElectronics.HexFileLib;

public enum HexRecordType
{
    Undefined = -1,
    Data = 0,
    EndOfFile = 1,
    ExtendedSegmentAddress = 2,
    StartSegmentAddress = 3,
    ExtendedLinearAddress = 4,
    StartLinearAddress = 5
}