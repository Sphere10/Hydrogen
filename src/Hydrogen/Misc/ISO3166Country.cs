using System.ComponentModel;

namespace Hydrogen;

public enum ISO3166Country {

	[CountryCodeAlpha2("AF")]
	[CountryCodeAlpha3("AFG")]
	[Description("Afghanistan")] Afghanistan = 004,

	[CountryCodeAlpha2("AL")]
	[CountryCodeAlpha3("ALB")]
	[Description("Albania")] Albania = 008,

	[CountryCodeAlpha2("DZ")]
	[CountryCodeAlpha3("DZA")]
	[Description("Algeria")] Algeria = 012,

	[CountryCodeAlpha2("AS")]
	[CountryCodeAlpha3("ASM")]
	[Description("American Samoa")] AmericanSamoa = 016,

	[CountryCodeAlpha2("AD")]
	[CountryCodeAlpha3("AND")]
	[Description("Andorra")] Andorra = 020,

	[CountryCodeAlpha2("AO")]
	[CountryCodeAlpha3("AGO")]
	[Description("Angola")] Angola = 024,

	[CountryCodeAlpha2("AI")]
	[CountryCodeAlpha3("AIA")]
	[Description("Anguilla")] Anguilla = 660,

	[CountryCodeAlpha2("AQ")]
	[CountryCodeAlpha3("ATA")]
	[Description("Antarctica")] Antarctica = 010,

	[CountryCodeAlpha2("AG")]
	[CountryCodeAlpha3("ATG")]
	[Description("Antigua and Barbuda")] AntiguaAndBarbuda = 028,

	[CountryCodeAlpha2("AR")]
	[CountryCodeAlpha3("ARG")]
	[Description("Argentina")] Argentina = 032,

	[CountryCodeAlpha2("AM")]
	[CountryCodeAlpha3("ARM")]
	[Description("Armenia")] Armenia = 051,

	[CountryCodeAlpha2("AW")]
	[CountryCodeAlpha3("ABW")]
	[Description("Aruba")] Aruba = 533,

	[CountryCodeAlpha2("AU")]
	[CountryCodeAlpha3("AUS")]
	[Description("Australia")] Australia = 036,

	[CountryCodeAlpha2("AT")]
	[CountryCodeAlpha3("AUT")]
	[Description("Austria")] Austria = 040,

	[CountryCodeAlpha2("AZ")]
	[CountryCodeAlpha3("AZE")]
	[Description("Azerbaijan")] Azerbaijan = 031,

	[CountryCodeAlpha2("BS")]
	[CountryCodeAlpha3("BHS")]
	[Description("Bahamas")] Bahamas = 044,

	[CountryCodeAlpha2("BH")]
	[CountryCodeAlpha3("BHR")]
	[Description("Bahrain")] Bahrain = 048,

	[CountryCodeAlpha2("BD")]
	[CountryCodeAlpha3("BGD")]
	[Description("Bangladesh")] Bangladesh = 050,

	[CountryCodeAlpha2("BB")]
	[CountryCodeAlpha3("BRB")]
	[Description("Barbados")] Barbados = 052,

	[CountryCodeAlpha2("BY")]
	[CountryCodeAlpha3("BLR")]
	[Description("Belarus")] Belarus = 112,

	[CountryCodeAlpha2("BE")]
	[CountryCodeAlpha3("BEL")]
	[Description("Belgium")] Belgium = 056,

	[CountryCodeAlpha2("BZ")]
	[CountryCodeAlpha3("BLZ")]
	[Description("Belize")] Belize = 084,

	[CountryCodeAlpha2("BJ")]
	[CountryCodeAlpha3("BEN")]
	[Description("Benin")] Benin = 204,

	[CountryCodeAlpha2("BM")]
	[CountryCodeAlpha3("BMU")]
	[Description("Bermuda")] Bermuda = 060,

	[CountryCodeAlpha2("BT")]
	[CountryCodeAlpha3("BTN")]
	[Description("Bhutan")] Bhutan = 064,

	[CountryCodeAlpha2("BO")]
	[CountryCodeAlpha3("BOL")]
	[Description("Bolivia, Plurinational State of")] Bolivia = 068,

	[CountryCodeAlpha2("BQ")]
	[CountryCodeAlpha3("BES")]
	[Description("Bonaire, Sint Eustatius and Saba")] Bonaire = 535,

	[CountryCodeAlpha2("BA")]
	[CountryCodeAlpha3("BIH")]
	[Description("Bosnia and Herzegovina")] BosniaAndHerzegovina = 070,

	[CountryCodeAlpha2("BW")]
	[CountryCodeAlpha3("BWA")]
	[Description("Botswana")] Botswana = 072,

	[CountryCodeAlpha2("BV")]
	[CountryCodeAlpha3("BVT")]
	[Description("Bouvet Island")] BouvetIsland = 074,

	[CountryCodeAlpha2("BR")]
	[CountryCodeAlpha3("BRA")]
	[Description("Brazil")] Brazil = 076,

	[CountryCodeAlpha2("IO")]
	[CountryCodeAlpha3("IOT")]
	[Description("British Indian Ocean Territory")] BritishIndianOceanTerritory = 086,

	[CountryCodeAlpha2("BN")]
	[CountryCodeAlpha3("BRN")]
	[Description("Brunei Darussalam")] BruneiDarussalam = 096,

	[CountryCodeAlpha2("BG")]
	[CountryCodeAlpha3("BGR")]
	[Description("Bulgaria")] Bulgaria = 100,

	[CountryCodeAlpha2("BF")]
	[CountryCodeAlpha3("BFA")]
	[Description("Burkina Faso")] BurkinaFaso = 854,

	[CountryCodeAlpha2("BI")]
	[CountryCodeAlpha3("BDI")]
	[Description("Burundi")] Burundi = 108,

	[CountryCodeAlpha2("CV")]
	[CountryCodeAlpha3("CPV")]
	[Description("Cabo Verde")] CaboVerde = 132,

	[CountryCodeAlpha2("KH")]
	[CountryCodeAlpha3("KHM")]
	[Description("Cambodia")] Cambodia = 116,

	[CountryCodeAlpha2("CM")]
	[CountryCodeAlpha3("CMR")]
	[Description("Cameroon")] Cameroon = 120,

	[CountryCodeAlpha2("CA")]
	[CountryCodeAlpha3("CAN")]
	[Description("Canada")] Canada = 124,

	[CountryCodeAlpha2("KY")]
	[CountryCodeAlpha3("CYM")]
	[Description("Cayman Islands")] CaymanIslands = 136,

	[CountryCodeAlpha2("CF")]
	[CountryCodeAlpha3("CAF")]
	[Description("Central African Republic")] CentralAfricanRepublic = 140,

	[CountryCodeAlpha2("TD")]
	[CountryCodeAlpha3("TCD")]
	[Description("Chad")] Chad = 148,

	[CountryCodeAlpha2("CL")]
	[CountryCodeAlpha3("CHL")]
	[Description("Chile")] Chile = 152,

	[CountryCodeAlpha2("CN")]
	[CountryCodeAlpha3("CHN")]
	[Description("China")] China = 156,

	[CountryCodeAlpha2("CX")]
	[CountryCodeAlpha3("CXR")]
	[Description("Christmas Island")] ChristmasIsland = 162,

	[CountryCodeAlpha2("CC")]
	[CountryCodeAlpha3("CCK")]
	[Description("Cocos (Keeling) Islands")] CocosIslands = 166,

	[CountryCodeAlpha2("CO")]
	[CountryCodeAlpha3("COL")]
	[Description("Colombia")] Colombia = 170,

	[CountryCodeAlpha2("KM")]
	[CountryCodeAlpha3("COM")]
	[Description("Comoros")] Comoros = 174,

	[CountryCodeAlpha2("CG")]
	[CountryCodeAlpha3("COG")]
	[Description("Congo")] Congo = 178,

	[CountryCodeAlpha2("CD")]
	[CountryCodeAlpha3("COD")]
	[Description("Congo, the Democratic Republic of the")] CongoRepublic = 180,

	[CountryCodeAlpha2("CK")]
	[CountryCodeAlpha3("COK")]
	[Description("Cook Islands")] CookIslands = 184,

	[CountryCodeAlpha2("CR")]
	[CountryCodeAlpha3("CRI")]
	[Description("Costa Rica")] CostaRica = 188,

	[CountryCodeAlpha2("CI")]
	[CountryCodeAlpha3("CIV")]
	[Description("Côte d'Ivoire")] CôtedIvoire = 384,

	[CountryCodeAlpha2("HR")]
	[CountryCodeAlpha3("HRV")]
	[Description("Croatia")] Croatia = 191,

	[CountryCodeAlpha2("CU")]
	[CountryCodeAlpha3("CUB")]
	[Description("Cuba")] Cuba = 192,

	[CountryCodeAlpha2("CW")]
	[CountryCodeAlpha3("CUW")]
	[Description("Curaçao")] Curaçao = 531,

	[CountryCodeAlpha2("CY")]
	[CountryCodeAlpha3("CYP")]
	[Description("Cyprus")] Cyprus = 196,

	[CountryCodeAlpha2("CZ")]
	[CountryCodeAlpha3("CZE")]
	[Description("Czechia")] Czechia = 203,

	[CountryCodeAlpha2("DK")]
	[CountryCodeAlpha3("DNK")]
	[Description("Denmark")] Denmark = 208,

	[CountryCodeAlpha2("DJ")]
	[CountryCodeAlpha3("DJI")]
	[Description("Djibouti")] Djibouti = 262,

	[CountryCodeAlpha2("DM")]
	[CountryCodeAlpha3("DMA")]
	[Description("Dominica")] Dominica = 212,

	[CountryCodeAlpha2("DO")]
	[CountryCodeAlpha3("DOM")]
	[Description("Dominican Republic")] DominicanRepublic = 214,

	[CountryCodeAlpha2("EC")]
	[CountryCodeAlpha3("ECU")]
	[Description("Ecuador")] Ecuador = 218,

	[CountryCodeAlpha2("EG")]
	[CountryCodeAlpha3("EGY")]
	[Description("Egypt")] Egypt = 818,

	[CountryCodeAlpha2("SV")]
	[CountryCodeAlpha3("SLV")]
	[Description("El Salvador")] ElSalvador = 222,

	[CountryCodeAlpha2("GQ")]
	[CountryCodeAlpha3("GNQ")]
	[Description("Equatorial Guinea")] EquatorialGuinea = 226,

	[CountryCodeAlpha2("ER")]
	[CountryCodeAlpha3("ERI")]
	[Description("Eritrea")] Eritrea = 232,

	[CountryCodeAlpha2("EE")]
	[CountryCodeAlpha3("EST")]
	[Description("Estonia")] Estonia = 233,

	[CountryCodeAlpha2("SZ")]
	[CountryCodeAlpha3("SWZ")]
	[Description("Eswatini")] Eswatini = 748,

	[CountryCodeAlpha2("ET")]
	[CountryCodeAlpha3("ETH")]
	[Description("Ethiopia")] Ethiopia = 231,

	[CountryCodeAlpha2("FK")]
	[CountryCodeAlpha3("FLK")]
	[Description("Falkland Islands (Malvinas)")] Malvinas = 238,

	[CountryCodeAlpha2("FO")]
	[CountryCodeAlpha3("FRO")]
	[Description("Faroe Islands")] FaroeIslands = 234,

	[CountryCodeAlpha2("FJ")]
	[CountryCodeAlpha3("FJI")]
	[Description("Fiji")] Fiji = 242,

	[CountryCodeAlpha2("FI")]
	[CountryCodeAlpha3("FIN")]
	[Description("Finland")] Finland = 246,

	[CountryCodeAlpha2("FR")]
	[CountryCodeAlpha3("FRA")]
	[Description("France")] France = 250,

	[CountryCodeAlpha2("GF")]
	[CountryCodeAlpha3("GUF")]
	[Description("French Guiana")] FrenchGuiana = 254,

	[CountryCodeAlpha2("PF")]
	[CountryCodeAlpha3("PYF")]
	[Description("French Polynesia")] FrenchPolynesia = 258,

	[CountryCodeAlpha2("TF")]
	[CountryCodeAlpha3("ATF")]
	[Description("French Southern Territories")] FrenchSouthernTerritories = 260,

	[CountryCodeAlpha2("GA")]
	[CountryCodeAlpha3("GAB")]
	[Description("Gabon")] Gabon = 266,

	[CountryCodeAlpha2("GM")]
	[CountryCodeAlpha3("GMB")]
	[Description("Gambia")] Gambia = 270,

	[CountryCodeAlpha2("GE")]
	[CountryCodeAlpha3("GEO")]
	[Description("Georgia")] Georgia = 268,

	[CountryCodeAlpha2("DE")]
	[CountryCodeAlpha3("DEU")]
	[Description("Germany")] Germany = 276,

	[CountryCodeAlpha2("GH")]
	[CountryCodeAlpha3("GHA")]
	[Description("Ghana")] Ghana = 288,

	[CountryCodeAlpha2("GI")]
	[CountryCodeAlpha3("GIB")]
	[Description("Gibraltar")] Gibraltar = 292,

	[CountryCodeAlpha2("GR")]
	[CountryCodeAlpha3("GRC")]
	[Description("Greece")] Greece = 300,

	[CountryCodeAlpha2("GL")]
	[CountryCodeAlpha3("GRL")]
	[Description("Greenland")] Greenland = 304,

	[CountryCodeAlpha2("GD")]
	[CountryCodeAlpha3("GRD")]
	[Description("Grenada")] Grenada = 308,

	[CountryCodeAlpha2("GP")]
	[CountryCodeAlpha3("GLP")]
	[Description("Guadeloupe")] Guadeloupe = 312,

	[CountryCodeAlpha2("GU")]
	[CountryCodeAlpha3("GUM")]
	[Description("Guam")] Guam = 316,

	[CountryCodeAlpha2("GT")]
	[CountryCodeAlpha3("GTM")]
	[Description("Guatemala")] Guatemala = 320,

	[CountryCodeAlpha2("GG")]
	[CountryCodeAlpha3("GGY")]
	[Description("Guernsey")] Guernsey = 831,

	[CountryCodeAlpha2("GN")]
	[CountryCodeAlpha3("GIN")]
	[Description("Guinea")] Guinea = 324,

	[CountryCodeAlpha2("GW")]
	[CountryCodeAlpha3("GNB")]
	[Description("Guinea-Bissau")] GuineaBissau = 624,

	[CountryCodeAlpha2("GY")]
	[CountryCodeAlpha3("GUY")]
	[Description("Guyana")] Guyana = 328,

	[CountryCodeAlpha2("HT")]
	[CountryCodeAlpha3("HTI")]
	[Description("Haiti")] Haiti = 332,

	[CountryCodeAlpha2("HM")]
	[CountryCodeAlpha3("HMD")]
	[Description("Heard Island and McDonald Islands")] HeardIslandAndMcDonaldIslands = 334,

	[CountryCodeAlpha2("VA")]
	[CountryCodeAlpha3("VAT")]
	[Description("Holy See")] HolySee = 336,

	[CountryCodeAlpha2("HN")]
	[CountryCodeAlpha3("HND")]
	[Description("Honduras")] Honduras = 340,

	[CountryCodeAlpha2("HK")]
	[CountryCodeAlpha3("HKG")]
	[Description("Hong Kong")] HongKong = 344,

	[CountryCodeAlpha2("HU")]
	[CountryCodeAlpha3("HUN")]
	[Description("Hungary")] Hungary = 348,

	[CountryCodeAlpha2("IS")]
	[CountryCodeAlpha3("ISL")]
	[Description("Iceland")] Iceland = 352,

	[CountryCodeAlpha2("IN")]
	[CountryCodeAlpha3("IND")]
	[Description("India")] India = 356,

	[CountryCodeAlpha2("ID")]
	[CountryCodeAlpha3("IDN")]
	[Description("Indonesia")] Indonesia = 360,

	[CountryCodeAlpha2("IR")]
	[CountryCodeAlpha3("IRN")]
	[Description("Iran, Islamic Republic of")] Iran = 364,

	[CountryCodeAlpha2("IQ")]
	[CountryCodeAlpha3("IRQ")]
	[Description("Iraq")] Iraq = 368,

	[CountryCodeAlpha2("IE")]
	[CountryCodeAlpha3("IRL")]
	[Description("Ireland")] Ireland = 372,

	[CountryCodeAlpha2("IM")]
	[CountryCodeAlpha3("IMN")]
	[Description("Isle of Man")] IsleOfMan = 833,

	[CountryCodeAlpha2("IL")]
	[CountryCodeAlpha3("ISR")]
	[Description("Israel")] Israel = 376,

	[CountryCodeAlpha2("IT")]
	[CountryCodeAlpha3("ITA")]
	[Description("Italy")] Italy = 380,

	[CountryCodeAlpha2("JM")]
	[CountryCodeAlpha3("JAM")]
	[Description("Jamaica")] Jamaica = 388,

	[CountryCodeAlpha2("JP")]
	[CountryCodeAlpha3("JPN")]
	[Description("Japan")] Japan = 392,

	[CountryCodeAlpha2("JE")]
	[CountryCodeAlpha3("JEY")]
	[Description("Jersey")] Jersey = 832,

	[CountryCodeAlpha2("JO")]
	[CountryCodeAlpha3("JOR")]
	[Description("Jordan")] Jordan = 400,

	[CountryCodeAlpha2("KZ")]
	[CountryCodeAlpha3("KAZ")]
	[Description("Kazakhstan")] Kazakhstan = 398,

	[CountryCodeAlpha2("KE")]
	[CountryCodeAlpha3("KEN")]
	[Description("Kenya")] Kenya = 404,

	[CountryCodeAlpha2("KI")]
	[CountryCodeAlpha3("KIR")]
	[Description("Kiribati")] Kiribati = 296,

	[CountryCodeAlpha2("KP")]
	[CountryCodeAlpha3("PRK")]
	[Description("North Korea, Democratic People's Republic of")] NorthKorea = 408,

	[CountryCodeAlpha2("KR")]
	[CountryCodeAlpha3("KOR")]
	[Description("South Korea, Republic of")] SouthKorea = 410,

	[CountryCodeAlpha2("KW")]
	[CountryCodeAlpha3("KWT")]
	[Description("Kuwait")] Kuwait = 414,

	[CountryCodeAlpha2("KG")]
	[CountryCodeAlpha3("KGZ")]
	[Description("Kyrgyzstan")] Kyrgyzstan = 417,

	[CountryCodeAlpha2("LA")]
	[CountryCodeAlpha3("LAO")]
	[Description("Lao People's Democratic Republic")] LaoPeopleDemocraticRepublic = 418,

	[CountryCodeAlpha2("LV")]
	[CountryCodeAlpha3("LVA")]
	[Description("Latvia")] Latvia = 428,

	[CountryCodeAlpha2("LB")]
	[CountryCodeAlpha3("LBN")]
	[Description("Lebanon")] Lebanon = 422,

	[CountryCodeAlpha2("LS")]
	[CountryCodeAlpha3("LSO")]
	[Description("Lesotho")] Lesotho = 426,

	[CountryCodeAlpha2("LR")]
	[CountryCodeAlpha3("LBR")]
	[Description("Liberia")] Liberia = 430,

	[CountryCodeAlpha2("LY")]
	[CountryCodeAlpha3("LBY")]
	[Description("Libya")] Libya = 434,

	[CountryCodeAlpha2("LI")]
	[CountryCodeAlpha3("LIE")]
	[Description("Liechtenstein")] Liechtenstein = 438,

	[CountryCodeAlpha2("LT")]
	[CountryCodeAlpha3("LTU")]
	[Description("Lithuania")] Lithuania = 440,

	[CountryCodeAlpha2("LU")]
	[CountryCodeAlpha3("LUX")]
	[Description("Luxembourg")] Luxembourg = 442,

	[CountryCodeAlpha2("MO")]
	[CountryCodeAlpha3("MAC")]
	[Description("Macao")] Macao = 446,

	[CountryCodeAlpha2("MG")]
	[CountryCodeAlpha3("MDG")]
	[Description("Madagascar")] Madagascar = 450,

	[CountryCodeAlpha2("MW")]
	[CountryCodeAlpha3("MWI")]
	[Description("Malawi")] Malawi = 454,

	[CountryCodeAlpha2("MY")]
	[CountryCodeAlpha3("MYS")]
	[Description("Malaysia")] Malaysia = 458,

	[CountryCodeAlpha2("MV")]
	[CountryCodeAlpha3("MDV")]
	[Description("Maldives")] Maldives = 462,

	[CountryCodeAlpha2("ML")]
	[CountryCodeAlpha3("MLI")]
	[Description("Mali")] Mali = 466,

	[CountryCodeAlpha2("MT")]
	[CountryCodeAlpha3("MLT")]
	[Description("Malta")] Malta = 470,

	[CountryCodeAlpha2("MH")]
	[CountryCodeAlpha3("MHL")]
	[Description("Marshall Islands")] MarshallIslands = 584,

	[CountryCodeAlpha2("MQ")]
	[CountryCodeAlpha3("MTQ")]
	[Description("Martinique")] Martinique = 474,

	[CountryCodeAlpha2("MR")]
	[CountryCodeAlpha3("MRT")]
	[Description("Mauritania")] Mauritania = 478,

	[CountryCodeAlpha2("MU")]
	[CountryCodeAlpha3("MUS")]
	[Description("Mauritius")] Mauritius = 480,

	[CountryCodeAlpha2("YT")]
	[CountryCodeAlpha3("MYT")]
	[Description("Mayotte")] Mayotte = 175,

	[CountryCodeAlpha2("MX")]
	[CountryCodeAlpha3("MEX")]
	[Description("Mexico")] Mexico = 484,

	[CountryCodeAlpha2("FM")]
	[CountryCodeAlpha3("FSM")]
	[Description("Micronesia, Federated States of")] Micronesia = 583,

	[CountryCodeAlpha2("MD")]
	[CountryCodeAlpha3("MDA")]
	[Description("Moldova, Republic of")] Moldova = 498,

	[CountryCodeAlpha2("MC")]
	[CountryCodeAlpha3("MCO")]
	[Description("Monaco")] Monaco = 492,

	[CountryCodeAlpha2("MN")]
	[CountryCodeAlpha3("MNG")]
	[Description("Mongolia")] Mongolia = 496,

	[CountryCodeAlpha2("ME")]
	[CountryCodeAlpha3("MNE")]
	[Description("Montenegro")] Montenegro = 499,

	[CountryCodeAlpha2("MS")]
	[CountryCodeAlpha3("MSR")]
	[Description("Montserrat")] Montserrat = 500,

	[CountryCodeAlpha2("MA")]
	[CountryCodeAlpha3("MAR")]
	[Description("Morocco")] Morocco = 504,

	[CountryCodeAlpha2("MZ")]
	[CountryCodeAlpha3("MOZ")]
	[Description("Mozambique")] Mozambique = 508,

	[CountryCodeAlpha2("MM")]
	[CountryCodeAlpha3("MMR")]
	[Description("Myanmar")] Myanmar = 104,

	[CountryCodeAlpha2("NA")]
	[CountryCodeAlpha3("NAM")]
	[Description("Namibia")] Namibia = 516,

	[CountryCodeAlpha2("NR")]
	[CountryCodeAlpha3("NRU")]
	[Description("Nauru")] Nauru = 520,

	[CountryCodeAlpha2("NP")]
	[CountryCodeAlpha3("NPL")]
	[Description("Nepal")] Nepal = 524,

	[CountryCodeAlpha2("NL")]
	[CountryCodeAlpha3("NLD")]
	[Description("Netherlands")] Netherlands = 528,

	[CountryCodeAlpha2("NC")]
	[CountryCodeAlpha3("NCL")]
	[Description("New Caledonia")] NewCaledonia = 540,

	[CountryCodeAlpha2("NZ")]
	[CountryCodeAlpha3("NZL")]
	[Description("New Zealand")] NewZealand = 554,

	[CountryCodeAlpha2("NI")]
	[CountryCodeAlpha3("NIC")]
	[Description("Nicaragua")] Nicaragua = 558,

	[CountryCodeAlpha2("NE")]
	[CountryCodeAlpha3("NER")]
	[Description("Niger")] Niger = 562,

	[CountryCodeAlpha2("NG")]
	[CountryCodeAlpha3("NGA")]
	[Description("Nigeria")] Nigeria = 566,

	[CountryCodeAlpha2("NU")]
	[CountryCodeAlpha3("NIU")]
	[Description("Niue")] Niue = 570,

	[CountryCodeAlpha2("NF")]
	[CountryCodeAlpha3("NFK")]
	[Description("Norfolk Island")] NorfolkIsland = 574,

	[CountryCodeAlpha2("MP")]
	[CountryCodeAlpha3("MNP")]
	[Description("Northern Mariana Islands")] NorthernMarianaIslands = 580,

	[CountryCodeAlpha2("MK")]
	[CountryCodeAlpha3("MKD")]
	[Description("North Macedonia")] NorthMacedonia = 807,

	[CountryCodeAlpha2("NO")]
	[CountryCodeAlpha3("NOR")]
	[Description("Norway")] Norway = 578,

	[CountryCodeAlpha2("OM")]
	[CountryCodeAlpha3("OMN")]
	[Description("Oman")] Oman = 512,

	[CountryCodeAlpha2("PK")]
	[CountryCodeAlpha3("PAK")]
	[Description("Pakistan")] Pakistan = 586,

	[CountryCodeAlpha2("PW")]
	[CountryCodeAlpha3("PLW")]
	[Description("Palau")] Palau = 585,

	[CountryCodeAlpha2("PS")]
	[CountryCodeAlpha3("PSE")]
	[Description("Palestine, State of")] Palestine = 275,

	[CountryCodeAlpha2("PA")]
	[CountryCodeAlpha3("PAN")]
	[Description("Panama")] Panama = 591,

	[CountryCodeAlpha2("PG")]
	[CountryCodeAlpha3("PNG")]
	[Description("Papua New Guinea")] PapuaNewGuinea = 598,

	[CountryCodeAlpha2("PY")]
	[CountryCodeAlpha3("PRY")]
	[Description("Paraguay")] Paraguay = 600,

	[CountryCodeAlpha2("PE")]
	[CountryCodeAlpha3("PER")]
	[Description("Peru")] Peru = 604,

	[CountryCodeAlpha2("PH")]
	[CountryCodeAlpha3("PHL")]
	[Description("Philippines")] Philippines = 608,

	[CountryCodeAlpha2("PN")]
	[CountryCodeAlpha3("PCN")]
	[Description("Pitcairn")] Pitcairn = 612,

	[CountryCodeAlpha2("PL")]
	[CountryCodeAlpha3("POL")]
	[Description("Poland")] Poland = 616,

	[CountryCodeAlpha2("PT")]
	[CountryCodeAlpha3("PRT")]
	[Description("Portugal")] Portugal = 620,

	[CountryCodeAlpha2("PR")]
	[CountryCodeAlpha3("PRI")]
	[Description("Puerto Rico")] PuertoRico = 630,

	[CountryCodeAlpha2("QA")]
	[CountryCodeAlpha3("QAT")]
	[Description("Qatar")] Qatar = 634,

	[CountryCodeAlpha2("RE")]
	[CountryCodeAlpha3("REU")]
	[Description("Réunion")] Réunion = 638,

	[CountryCodeAlpha2("RO")]
	[CountryCodeAlpha3("ROU")]
	[Description("Romania")] Romania = 642,

	[CountryCodeAlpha2("RU")]
	[CountryCodeAlpha3("RUS")]
	[Description("Russian Federation")] RussianFederation = 643,

	[CountryCodeAlpha2("RW")]
	[CountryCodeAlpha3("RWA")]
	[Description("Rwanda")] Rwanda = 646,

	[CountryCodeAlpha2("BL")]
	[CountryCodeAlpha3("BLM")]
	[Description("Saint Barthélemy")] SaintBarthélemy = 652,

	[CountryCodeAlpha2("SH")]
	[CountryCodeAlpha3("SHN")]
	[Description("Saint Helena, Ascension and Tristan da Cunha")] SaintHelena = 654,

	[CountryCodeAlpha2("KN")]
	[CountryCodeAlpha3("KNA")]
	[Description("Saint Kitts and Nevis")] SaintKittsAndNevis = 659,

	[CountryCodeAlpha2("LC")]
	[CountryCodeAlpha3("LCA")]
	[Description("Saint Lucia")] SaintLucia = 662,

	[CountryCodeAlpha2("MF")]
	[CountryCodeAlpha3("MAF")]
	[Description("Saint Martin (French part)")] SaintMartin = 663,

	[CountryCodeAlpha2("PM")]
	[CountryCodeAlpha3("SPM")]
	[Description("Saint Pierre and Miquelon")] SaintPierreAndMiquelon = 666,

	[CountryCodeAlpha2("VC")]
	[CountryCodeAlpha3("VCT")]
	[Description("Saint Vincent and the Grenadines")] SaintVincentAndTheGrenadines = 670,

	[CountryCodeAlpha2("WS")]
	[CountryCodeAlpha3("WSM")]
	[Description("Samoa")] Samoa = 882,

	[CountryCodeAlpha2("SM")]
	[CountryCodeAlpha3("SMR")]
	[Description("San Marino")] SanMarino = 674,

	[CountryCodeAlpha2("ST")]
	[CountryCodeAlpha3("STP")]
	[Description("Sao Tome and Principe")] SaoTomeAndPrincipe = 678,

	[CountryCodeAlpha2("SA")]
	[CountryCodeAlpha3("SAU")]
	[Description("Saudi Arabia")] SaudiArabia = 682,

	[CountryCodeAlpha2("SN")]
	[CountryCodeAlpha3("SEN")]
	[Description("Senegal")] Senegal = 686,

	[CountryCodeAlpha2("RS")]
	[CountryCodeAlpha3("SRB")]
	[Description("Serbia")] Serbia = 688,

	[CountryCodeAlpha2("SC")]
	[CountryCodeAlpha3("SYC")]
	[Description("Seychelles")] Seychelles = 690,

	[CountryCodeAlpha2("SL")]
	[CountryCodeAlpha3("SLE")]
	[Description("Sierra Leone")] SierraLeone = 694,

	[CountryCodeAlpha2("SG")]
	[CountryCodeAlpha3("SGP")]
	[Description("Singapore")] Singapore = 702,

	[CountryCodeAlpha2("SX")]
	[CountryCodeAlpha3("SXM")]
	[Description("Sint Maarten (Dutch part)")] SintMaarten = 534,

	[CountryCodeAlpha2("SK")]
	[CountryCodeAlpha3("SVK")]
	[Description("Slovakia")] Slovakia = 703,

	[CountryCodeAlpha2("SI")]
	[CountryCodeAlpha3("SVN")]
	[Description("Slovenia")] Slovenia = 705,

	[CountryCodeAlpha2("SB")]
	[CountryCodeAlpha3("SLB")]
	[Description("Solomon Islands")] SolomonIslands = 090,

	[CountryCodeAlpha2("SO")]
	[CountryCodeAlpha3("SOM")]
	[Description("Somalia")] Somalia = 706,

	[CountryCodeAlpha2("ZA")]
	[CountryCodeAlpha3("ZAF")]
	[Description("South Africa")] SouthAfrica = 710,

	[CountryCodeAlpha2("GS")]
	[CountryCodeAlpha3("SGS")]
	[Description("South Georgia and the South Sandwich Islands")] SouthGeorgia = 239,

	[CountryCodeAlpha2("SS")]
	[CountryCodeAlpha3("SSD")]
	[Description("South Sudan")] SouthSudan = 728,

	[CountryCodeAlpha2("ES")]
	[CountryCodeAlpha3("ESP")]
	[Description("Spain")] Spain = 724,

	[CountryCodeAlpha2("LK")]
	[CountryCodeAlpha3("LKA")]
	[Description("Sri Lanka")] SriLanka = 144,

	[CountryCodeAlpha2("SD")]
	[CountryCodeAlpha3("SDN")]
	[Description("Sudan")] Sudan = 729,

	[CountryCodeAlpha2("SR")]
	[CountryCodeAlpha3("SUR")]
	[Description("Suriname")] Suriname = 740,

	[CountryCodeAlpha2("SJ")]
	[CountryCodeAlpha3("SJM")]
	[Description("Svalbard and Jan Mayen")] SvalbardAndJanMayen = 744,

	[CountryCodeAlpha2("SE")]
	[CountryCodeAlpha3("SWE")]
	[Description("Sweden")] Sweden = 752,

	[CountryCodeAlpha2("CH")]
	[CountryCodeAlpha3("CHE")]
	[Description("Switzerland")] Switzerland = 756,

	[CountryCodeAlpha2("SY")]
	[CountryCodeAlpha3("SYR")]
	[Description("Syrian Arab Republic")] SyrianArabRepublic = 760,

	[CountryCodeAlpha2("TW")]
	[CountryCodeAlpha3("TWN")]
	[Description("Taiwan")] Taiwan = 158,

	[CountryCodeAlpha2("TJ")]
	[CountryCodeAlpha3("TJK")]
	[Description("Tajikistan")] Tajikistan = 762,

	[CountryCodeAlpha2("TZ")]
	[CountryCodeAlpha3("TZA")]
	[Description("Tanzania, United Republic of")] Tanzania = 834,

	[CountryCodeAlpha2("TH")]
	[CountryCodeAlpha3("THA")]
	[Description("Thailand")] Thailand = 764,

	[CountryCodeAlpha2("TL")]
	[CountryCodeAlpha3("TLS")]
	[Description("Timor-Leste")] TimorLeste = 626,

	[CountryCodeAlpha2("TG")]
	[CountryCodeAlpha3("TGO")]
	[Description("Togo")] Togo = 768,

	[CountryCodeAlpha2("TK")]
	[CountryCodeAlpha3("TKL")]
	[Description("Tokelau")] Tokelau = 772,

	[CountryCodeAlpha2("TO")]
	[CountryCodeAlpha3("TON")]
	[Description("Tonga")] Tonga = 776,

	[CountryCodeAlpha2("TT")]
	[CountryCodeAlpha3("TTO")]
	[Description("Trinidad and Tobago")] TrinidadAndTobago = 780,

	[CountryCodeAlpha2("TN")]
	[CountryCodeAlpha3("TUN")]
	[Description("Tunisia")] Tunisia = 788,

	[CountryCodeAlpha2("TR")]
	[CountryCodeAlpha3("TUR")]
	[Description("Turkey")] Turkey = 792,

	[CountryCodeAlpha2("TM")]
	[CountryCodeAlpha3("TKM")]
	[Description("Turkmenistan")] Turkmenistan = 795,

	[CountryCodeAlpha2("TC")]
	[CountryCodeAlpha3("TCA")]
	[Description("Turks and Caicos Islands")] TurksAndCaicosIslands = 796,

	[CountryCodeAlpha2("TV")]
	[CountryCodeAlpha3("TUV")]
	[Description("Tuvalu")] Tuvalu = 798,

	[CountryCodeAlpha2("UG")]
	[CountryCodeAlpha3("UGA")]
	[Description("Uganda")] Uganda = 800,

	[CountryCodeAlpha2("UA")]
	[CountryCodeAlpha3("UKR")]
	[Description("Ukraine")] Ukraine = 804,

	[CountryCodeAlpha2("AE")]
	[CountryCodeAlpha3("ARE")]
	[Description("United Arab Emirates")] UnitedArabEmirates = 784,

	[CountryCodeAlpha2("GB")]
	[CountryCodeAlpha3("GBR")]
	[Description("United Kingdom of Great Britain and Northern Ireland")] UnitedKingdom = 826,

	[CountryCodeAlpha2("US")]
	[CountryCodeAlpha3("USA")]
	[Description("United States of America")] UnitedStatesOfAmerica = 840,

	[CountryCodeAlpha2("UM")]
	[CountryCodeAlpha3("UMI")]
	[Description("United States Minor Outlying Islands")] UnitedStatesMinorOutlyingIslands = 581,

	[CountryCodeAlpha2("UY")]
	[CountryCodeAlpha3("URY")]
	[Description("Uruguay")] Uruguay = 858,

	[CountryCodeAlpha2("UZ")]
	[CountryCodeAlpha3("UZB")]
	[Description("Uzbekistan")] Uzbekistan = 860,

	[CountryCodeAlpha2("VU")]
	[CountryCodeAlpha3("VUT")]
	[Description("Vanuatu")] Vanuatu = 548,

	[CountryCodeAlpha2("VE")]
	[CountryCodeAlpha3("VEN")]
	[Description("Venezuela, Bolivarian Republic of")] Venezuela = 862,

	[CountryCodeAlpha2("VN")]
	[CountryCodeAlpha3("VNM")]
	[Description("Viet Nam")] Vietnam = 704,

	[CountryCodeAlpha2("VG")]
	[CountryCodeAlpha3("VGB")]
	[Description("Virgin Islands, British")] VirginIslandsBritish = 092,

	[CountryCodeAlpha2("VI")]
	[CountryCodeAlpha3("VIR")]
	[Description("Virgin Islands, U.S.")] VirginIslandsUS = 850,

	[CountryCodeAlpha2("WF")]
	[CountryCodeAlpha3("WLF")]
	[Description("Wallis and Futuna")] WallisAndFutuna = 876,

	[CountryCodeAlpha2("EH")]
	[CountryCodeAlpha3("ESH")]
	[Description("Western Sahara")] WesternSahara = 732,

	[CountryCodeAlpha2("YE")]
	[CountryCodeAlpha3("YEM")]
	[Description("Yemen")] Yemen = 887,

	[CountryCodeAlpha2("ZM")]
	[CountryCodeAlpha3("ZMB")]
	[Description("Zambia")] Zambia = 894,

	[CountryCodeAlpha2("ZW")]
	[CountryCodeAlpha3("ZWE")]
	[Description("Zimbabwe")] Zimbabwe = 716,

	[CountryCodeAlpha2("AX")]
	[CountryCodeAlpha3("ALA")]
	[Description("Åland Islands")] ÅlandIslands = 248,

}