namespace PoeHUD.Poe
{
	public class AreaTemplate : RemoteMemoryObject
	{
		public string RawName
		{
			get
			{
				return this.M.ReadStringU(this.M.ReadInt(this.Address), 256, true);
			}
		}
		public string Name
		{
			get
			{
				return this.M.ReadStringU(this.M.ReadInt(this.Address + 4), 256, true);
			}
		}
		public int Act
		{
			get
			{
				return this.M.ReadInt(this.Address + 8);
			}
		}
		public bool IsTown
		{
			get
			{
				return (this.M.ReadInt(this.Address + 12) & 1) == 1;
			}
		}
		public bool HasWaypoint
		{
			get
			{
				return (this.M.ReadInt(this.Address + 13) & 1) == 1;
			}
		}

		public int NominalLevel
		{
			get
			{
				return (this.M.ReadInt(this.Address + 0x16));
			}
		}
	}


/*
00000000 POE_Area struc ; (sizeof=0xEB)
00000000 zoneCode dd ?                           ; offset
00000004 zoneName dd ?                           ; offset
00000008 act dd ?
0000000C hasWp db ?
0000000D isTown db ?
0000000E cntLinkedZones dd ?
00000012 lpLinkedZones dd ?                      ; offset
00000016 Level dd ?                              ; base 10
0000001A field_1A dd ?
0000001E field_1E db ?
0000001F hashCode dd ?                           ; base 10
00000023 field_23 dd ?                           ; base 10
00000027 field_27 dd ?
0000002B lpLoadingScreen dd ?                    ; offset
0000002F field_2F dd ?                           ; base 10
00000033 field_33 dd ?
00000037 lpCntXZ dd ?                            ; offset
0000003B field_3B dd ?
0000003F cnt_p1 dd ?
00000043 lpTopologies dd ?                       ; offset (00000000)
00000047 lpTownArea dd ?                         ; offset
0000004B lpAllDificulties dd ?                   ; offset (00000000)
0000004F lplpDifficulty dd ?                     ; offset
00000053 field_53 dd ?                           ; base 10
00000057 field_57 dd ?                           ; base 10
0000005B field_5B dd ?                           ; base 10
0000005F field_5F dd ?                           ; base 10
00000063 field_63 dd ?                           ; base 10
00000067 field_67 dd ?                           ; offset (00000000)
0000006B field_6B dd ?
0000006F area_achievements1 dd ?                 ; offset (00000000)
00000073 field_73 dd ?
00000077 field_77 dd ?
0000007B area_achievements2 dd ?                 ; offset (00000000)
0000007F field_7F dd ?
00000083 area_achievements3 dd ?                 ; offset (00000000)
00000087 field_87 db ?
00000088 field_88 dd ?
0000008C area_achievements4 dd ?                 ; offset (00000000)
00000090 field_90 dd ?                           ; base 10
00000094 field_94 dd ?                           ; base 10
00000098 field_98 dd ?
0000009C field_9C dd ?
000000A0 field_A0 dd ?
000000A4 possibleVaal dd ?                       ; offset (00000000)
000000A8 possibleVaal2 dd ?                      ; offset (00000000)
000000AC field_AC dd ?
000000B0 field_B0 dd ?
000000B4 field_B4 dd ?
000000B8 lplpSideArea dd ?                       ; offset
000000BC field_BC dd ?                           ; base 10
000000C0 field_C0 dd ?                           ; base 10
000000C4 field_C4 dd ?                           ; base 10
000000C8 field_C8 dd ?                           ; base 10
000000CC zoneShortStruct dd ?                    ; offset
000000D0 field_D0 db ?
000000D1 field_D1 dd ?                           ; base 10
000000D5 field_D5 dd ?                           ; base 10
000000D9 field_D9 dd ?
000000DD field_DD dd ?                           ; offset
000000E1 field_E1 dd ?                           ; base 10
000000E5 field_E5 dd ?                           ; base 16
000000E9 field_E9 dw ?
000000EB POE_Area ends
	 * */

}
