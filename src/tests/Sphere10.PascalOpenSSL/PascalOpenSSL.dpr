program PascalOpenSSL;

{$APPTYPE CONSOLE}
{$R *.res}

uses
  SysUtils,
  UOpenSSL in 'UOpenSSL.pas',
  UPascalOpenSSL in 'UPascalOpenSSL.pas';

var
  OperationType, CurveType, MessageDigest, Result: String;
  PrivateKey: String;
  XCoord, YCoord, DerSig: String;

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
        Result := TPascalOpenSSL.ECDSAVerifyWrapper(CurveType, XCoord, YCoord,
          DerSig, MessageDigest);
        WriteLn(Result);
      end;

    end;
  except
    on E: Exception do
      WriteLn(E.ClassName, ': ', E.Message);
  end;

end.
