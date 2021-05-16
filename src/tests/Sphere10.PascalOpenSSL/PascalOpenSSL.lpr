program PascalOpenSSL;

{$IFDEF FPC}
{$MODE DELPHI}
{$ENDIF}

uses
  SysUtils,
  UOpenSSL in 'UOpenSSL.pas',
  UPascalOpenSSL in 'UPascalOpenSSL.pas';

type
  TCmdLineSwitchType = (clstValueNextParam, clstValueAppended);
  TCmdLineSwitchTypes = set of TCmdLineSwitchType;

  function FindCmdLineSwitch(const Switch: string; var Value: string;
    IgnoreCase: boolean = True; const SwitchTypes: TCmdLineSwitchTypes =
    [clstValueNextParam, clstValueAppended]): boolean; overload;
  type
    TCompareProc = function(const S1, S2: string): boolean;
  var
    Param: string;
    I, ValueOfs, SwitchLen, ParamLen: integer;
    SameSwitch: TCompareProc;
  begin
    Result := False;
    Value := '';
    if IgnoreCase then
      SameSwitch := SameText
    else
      SameSwitch := SameStr;
    SwitchLen := Switch.Length;

    for I := 1 to ParamCount do
    begin
      Param := ParamStr(I);
      if CharInSet(Param.Chars[0], SwitchChars) and
        SameSwitch(Param.SubString(1, SwitchLen), Switch) then
      begin
        ParamLen := Param.Length;
        // Look for an appended value if the param is longer than the switch
        if (ParamLen > SwitchLen + 1) then
        begin
          // If not looking for appended value switches then this is not a matching switch
          if not (clstValueAppended in SwitchTypes) then
            Continue;
          ValueOfs := SwitchLen + 1;
          if Param.Chars[ValueOfs] = ':' then
            Inc(ValueOfs);
          Value := Param.SubString(ValueOfs, MaxInt);
        end
        // If the next param is not a switch, then treat it as the value
        else if (clstValueNextParam in SwitchTypes) and (I < ParamCount) and
          not CharInSet(ParamStr(I + 1).Chars[0], SwitchChars) then
          Value := ParamStr(I + 1);
        Result := True;
        Break;
      end;
    end;
  end;

var
  OperationType, CurveType, MessageDigest, Result: string;
  PrivateKey: string;
  XCoord, YCoord, DerSig: string;

begin
  try
    { TODO -oUser -cConsole Main : Insert code here }
    if ParamCount > 1 then
    begin
      if not FindCmdLineSwitch('operationtype', OperationType) then
        Exit;
      if not FindCmdLineSwitch('curvetype', CurveType) then
        Exit;
      if not FindCmdLineSwitch('messagedigest', MessageDigest) then
        Exit;
      if OperationType = 'SIGN' then
      begin
        if not FindCmdLineSwitch('privatekey', PrivateKey) then
          Exit;
        Result := TPascalOpenSSL.ECDSASignWrapper(CurveType, PrivateKey,
          MessageDigest);
        WriteLn(Result);
      end
      else if OperationType = 'VERIFY' then
      begin
        if not FindCmdLineSwitch('xcoord', XCoord) then
          Exit;
        if not FindCmdLineSwitch('ycoord', YCoord) then
          Exit;
        if not FindCmdLineSwitch('dersig', DerSig) then
          Exit;
        Result := TPascalOpenSSL.ECDSAVerifyWrapper(CurveType, XCoord,
          YCoord, DerSig, MessageDigest);
        WriteLn(Result);
      end;

    end;
  except
    on E: Exception do
      WriteLn(E.ClassName, ': ', E.Message);
  end;

end.
