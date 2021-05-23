unit UPascalOpenSSL;

{$IFDEF FPC}
{$MODE DELPHI}
{$ENDIF}

interface

uses
  Classes,
  SysUtils,
  TypInfo,
  UOpenSSL;

resourcestring
  SInvalidCurveTypeSelected = 'Invalid CurveType Selected "%d".';

type
{$SCOPEDENUMS ON}
  TCurveType = (SECP256K1, SECP384R1, SECP521R1, SECT283K1);
{$SCOPEDENUMS OFF}

  { TECDSA_SIG is a Eliptic Curve signature }
type
  TECDSA_SIG = record
    R: TBytes;
    S: TBytes;
    DerSig: TBytes;
  end;

  { TECDSA_Public is a public key information }
  TECDSA_Public = record
    ECOpenSSLNID: Word;
    X: TBytes;
    Y: TBytes;
  end;

  { TECPrivateKeyInfo is a private key information }
  TECPrivateKeyInfo = record
    ECOpenSSLNID: Word;
    ECKeyPtr: Pointer;
  end;

type
  TPascalOpenSSL = class sealed(TObject)

  strict private

    class var

      FInitialized: Boolean;

    class function GetCurveTypeNumericValue(ACurveType: TCurveType)
      : Word; static;
    class function GetCurveTypeFromName(AName: String): TCurveType; static;
    class function DoECDSASignInternal(APEC_KEY: PEC_KEY;
      const AMessage: TBytes; var ASignature: TECDSA_SIG): Boolean; static;
    class function DoECDSAVerifyInternal(const APublicKey: TECDSA_Public;
      const ADerSignature, AMessage: TBytes): Boolean; static;

    class function SetPrivateKeyFromHexString(AEC_OpenSSL_NID: Word;
      const AHex: String; var APrivKey: TECPrivateKeyInfo): Boolean; static;

    class function DerEncodeECDSASig(const ASignature: PECDSA_SIG)
      : TBytes; static;
    class function DerDecodeECDSASig(const ADerSig: TBytes): PECDSA_SIG; static;

    class function BytesToHex(const ABytes: TBytes): String; static;
    class function HexToBytes(AHex: String): TBytes; static;

    class function DoECDSASign(const ACurveType: TCurveType;
      const APrivateKey, AMessage: TBytes): String; static;

    class function DoECDSAVerify(const ACurveType: TCurveType;
      const XCoord, YCoord, ADerSig, AMessage: TBytes): String; static;

    class procedure InitCrypto; static;
    class procedure DoInit; static;

    class constructor PascalOpenSSL();

  public

    class function ECDSASignWrapper(const ACurveType: String;
      const APrivateKey, AMessage: String): String; static;

    class function ECDSAVerifyWrapper(const ACurveType: String;
      const XCoord, YCoord, DerSig, AMessage: String): String; static;
  end;

implementation

class constructor TPascalOpenSSL.PascalOpenSSL;
begin
  TPascalOpenSSL.InitCrypto();
end;

class procedure TPascalOpenSSL.DoInit;
var
  err: String;
  c: Cardinal;
Begin
  if not(FInitialized) then
  begin
    FInitialized := True;
    If not InitSSLFunctions then
    begin
      err := 'Cannot load OpenSSL library ' + SSL_C_LIB;
      WriteLn('OpenSSL' + err);
      raise Exception.Create(err);
    end;

    If not Assigned(OpenSSL_version_num) then
    begin
      err := 'OpenSSL library is not v1.1 version: ' + SSL_C_LIB;
      WriteLn('OpenSSL' + err);
      raise Exception.Create(err);
    end;
    c := OpenSSL_version_num;
    if (c < $10100000) or (c > $1010FFFF) then
    begin
      err := 'OpenSSL library is not v1.1 version (' + IntToHex(c, 8) + '): ' +
        SSL_C_LIB;
      WriteLn('OpenSSL' + err);
      raise Exception.Create(err);
    end;
  end;
end;

class function TPascalOpenSSL.ECDSASignWrapper(const ACurveType, APrivateKey,
  AMessage: String): String;
begin
  Result := DoECDSASign(GetCurveTypeFromName(ACurveType),
    HexToBytes(APrivateKey), HexToBytes(AMessage));
end;

class function TPascalOpenSSL.ECDSAVerifyWrapper(const ACurveType, XCoord,
  YCoord, DerSig, AMessage: String): String;
begin
  Result := DoECDSAVerify(GetCurveTypeFromName(ACurveType), HexToBytes(XCoord),
    HexToBytes(YCoord), HexToBytes(DerSig), HexToBytes(AMessage));
end;

class function TPascalOpenSSL.GetCurveTypeFromName(AName: String): TCurveType;
begin
  Result := TCurveType(GetEnumValue(TypeInfo(TCurveType), AName));
end;

class function TPascalOpenSSL.GetCurveTypeNumericValue
  (ACurveType: TCurveType): Word;
begin
  case ACurveType of
    TCurveType.SECP256K1:
      Result := 714;
    TCurveType.SECP384R1:
      Result := 715;
    TCurveType.SECP521R1:
      Result := 716;
    TCurveType.SECT283K1:
      Result := 729
  else
    raise EArgumentOutOfRangeException.CreateResFmt(@SInvalidCurveTypeSelected,
      [Ord(ACurveType)]);
  end;

end;

class function TPascalOpenSSL.HexToBytes(AHex: String): TBytes;
begin
  AHex := LowerCase(AHex);
  Assert(Length(AHex) and 1 = 0);
  SetLength(Result, Length(AHex) shr 1);
  HexToBin(PChar(AHex), PChar(Result), Length(AHex));
end;

class function TPascalOpenSSL.BytesToHex(const ABytes: TBytes): String;
begin
  SetLength(Result, Length(ABytes) * 2);
  BinToHex(PChar(ABytes), PChar(Result), Length(ABytes));
end;

class procedure TPascalOpenSSL.InitCrypto;
begin
  // Load OpenSSL
  if Not LoadSSLCrypt then
    raise Exception.Create('Cannot load ' + SSL_C_LIB + #10 +
      'To use this software make sure this file is available on you system or reinstall the application');
  DoInit;
end;

class function TPascalOpenSSL.DerDecodeECDSASig(const ADerSig: TBytes)
  : PECDSA_SIG;
var
  P: PAnsiChar;
begin
  P := PAnsiChar(ADerSig);
  Result := d2i_ECDSA_SIG(Nil, @P, Length(ADerSig));
  if Result = Nil then
    raise Exception.Create('DER Signature Decoding Failed');
end;

class function TPascalOpenSSL.DerEncodeECDSASig(const ASignature
  : PECDSA_SIG): TBytes;
var
  SigSize: LongInt;
  P: PAnsiChar;
begin
  SigSize := i2d_ECDSA_SIG(ASignature, Nil);
  SetLength(Result, SigSize);
  FillChar(PByte(Result)^, SigSize, -1); // added for debugging purposes
  P := PAnsiChar(Result);
  i2d_ECDSA_SIG(ASignature, @P);
end;

class function TPascalOpenSSL.SetPrivateKeyFromHexString(AEC_OpenSSL_NID: Word;
  const AHex: String; var APrivKey: TECPrivateKeyInfo): Boolean;
var
  bn: PBIGNUM;
  ctx: PBN_CTX;
  pub_key: PEC_POINT;
  tmp_ansistring: RawByteString;
begin
  Result := False;
  bn := BN_new;
  try
    tmp_ansistring := RawByteString(AHex);
    if BN_hex2bn(@bn, PAnsiChar(tmp_ansistring)) = 0 then
      Raise Exception.Create
        ('Invalid hexa string to convert to Hexadecimal value');

    if Assigned(APrivKey.ECKeyPtr) then
      EC_KEY_free(APrivKey.ECKeyPtr);
    APrivKey.ECKeyPtr := Nil;

    APrivKey.ECOpenSSLNID := AEC_OpenSSL_NID;
    APrivKey.ECKeyPtr := EC_KEY_new_by_curve_name(AEC_OpenSSL_NID);
    If Not Assigned(APrivKey.ECKeyPtr) then
      Exit;
    if EC_KEY_set_private_key(APrivKey.ECKeyPtr, bn) <> 1 then
      raise Exception.Create('Invalid num to set as private key');

    ctx := BN_CTX_new;
    pub_key := EC_POINT_new(EC_KEY_get0_group(APrivKey.ECKeyPtr));
    try
      if EC_POINT_mul(EC_KEY_get0_group(APrivKey.ECKeyPtr), pub_key, bn, nil,
        nil, ctx) <> 1 then
        raise Exception.Create('Error obtaining public key');
      EC_KEY_set_public_key(APrivKey.ECKeyPtr, pub_key);
    finally
      BN_CTX_free(ctx);
      EC_POINT_free(pub_key);
    end;
  finally
    BN_free(bn);
  end;
  Result := True;
end;

class function TPascalOpenSSL.DoECDSASign(const ACurveType: TCurveType;
  const APrivateKey, AMessage: TBytes): String;
var
  PrivKey: TECPrivateKeyInfo;
  ECOpenSSLNID: Word;
  Sig: TECDSA_SIG;
begin
  Result := '';
  Sig := Default (TECDSA_SIG);
  PrivKey := Default (TECPrivateKeyInfo);
  ECOpenSSLNID := GetCurveTypeNumericValue(ACurveType);
  if SetPrivateKeyFromHexString(ECOpenSSLNID, BytesToHex(APrivateKey), PrivKey)
  then
  begin
    if DoECDSASignInternal(PrivKey.ECKeyPtr, AMessage, Sig) then
    begin
      Result := BytesToHex(Sig.DerSig);
    end
    else
    begin
      raise Exception.Create('Error Signing Message');
    end;
  end
  else
  begin
    raise Exception.Create('Error Setting Private Key');
  end;

end;

class function TPascalOpenSSL.DoECDSAVerify(const ACurveType: TCurveType;
  const XCoord, YCoord, ADerSig, AMessage: TBytes): String;
var
  PubKey: TECDSA_Public;
  ECOpenSSLNID: Word;
begin
  Result := '';
  PubKey := Default (TECDSA_Public);
  ECOpenSSLNID := GetCurveTypeNumericValue(ACurveType);
  PubKey.ECOpenSSLNID := ECOpenSSLNID;
  PubKey.X := XCoord;
  PubKey.Y := YCoord;
  Result := BoolToStr(DoECDSAVerifyInternal(PubKey, ADerSig, AMessage), True);
end;

class function TPascalOpenSSL.DoECDSASignInternal(APEC_KEY: PEC_KEY;
  const AMessage: TBytes; var ASignature: TECDSA_SIG): Boolean;
var
  PECS: PECDSA_SIG;
  P: PAnsiChar;
  I: Int32;
begin
  PECS := ECDSA_do_sign(PAnsiChar(@AMessage[Low(AMessage)]), Length(AMessage),
    APEC_KEY);
  Try
    if PECS = Nil then
      raise Exception.Create('Error Signing');

    I := BN_num_bytes(PECS^._r);
    SetLength(ASignature.R, I);
    P := @ASignature.R[Low(ASignature.R)];
    I := BN_bn2bin(PECS^._r, P);

    I := BN_num_bytes(PECS^._s);
    SetLength(ASignature.S, I);
    P := @ASignature.S[Low(ASignature.S)];
    I := BN_bn2bin(PECS^._s, P);
    ASignature.DerSig := DerEncodeECDSASig(PECS);
  Finally
    ECDSA_SIG_free(PECS);
  End;
  Result := True;
end;

class function TPascalOpenSSL.DoECDSAVerifyInternal(const APublicKey
  : TECDSA_Public; const ADerSignature, AMessage: TBytes): Boolean;
var
  BNx, BNy: PBIGNUM;
  ECG: PEC_GROUP;
  ctx: PBN_CTX;
  pub_key: PEC_POINT;
  PECS: PECDSA_SIG;
  PK: PEC_KEY;
begin
  BNx := BN_bin2bn(PAnsiChar(APublicKey.X), Length(APublicKey.X), Nil);
  BNy := BN_bin2bn(PAnsiChar(APublicKey.Y), Length(APublicKey.Y), Nil);

  ECG := EC_GROUP_new_by_curve_name(APublicKey.ECOpenSSLNID);
  pub_key := EC_POINT_new(ECG);
  ctx := BN_CTX_new;
  if EC_POINT_set_affine_coordinates_GFp(ECG, pub_key, BNx, BNy, ctx) = 1 then
  begin
    PECS := DerDecodeECDSASig(ADerSignature);
    try
      PK := EC_KEY_new_by_curve_name(APublicKey.ECOpenSSLNID);
      EC_KEY_set_public_key(PK, pub_key);
      case ECDSA_do_verify(@AMessage[Low(AMessage)], Length(AMessage),
        PECS, PK) of
        1:
          Result := True;
        0:
          Result := False;
      Else
        raise Exception.Create('Error on Verify ' + CaptureLastSSLError);
      End;
      EC_KEY_free(PK);
    finally
      ECDSA_SIG_free(PECS);
    end;
  end
  else
  begin
    Result := False;
  end;
  BN_CTX_free(ctx);
  EC_POINT_free(pub_key);
  EC_GROUP_free(ECG);
  BN_free(BNx);
  BN_free(BNy);
end;

end.
