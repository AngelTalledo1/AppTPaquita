; ModuleID = 'marshal_methods.x86.ll'
source_filename = "marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [352 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [698 x i32] [
	i32 2616222, ; 0: System.Net.NetworkInformation.dll => 0x27eb9e => 67
	i32 10166715, ; 1: System.Net.NameResolution.dll => 0x9b21bb => 66
	i32 15721112, ; 2: System.Runtime.Intrinsics.dll => 0xefe298 => 107
	i32 32687329, ; 3: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 262
	i32 34715100, ; 4: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 296
	i32 34839235, ; 5: System.IO.FileSystem.DriveInfo => 0x2139ac3 => 47
	i32 39485524, ; 6: System.Net.WebSockets.dll => 0x25a8054 => 79
	i32 42639949, ; 7: System.Threading.Thread => 0x28aa24d => 144
	i32 57725457, ; 8: it\Microsoft.Data.SqlClient.resources => 0x370d211 => 307
	i32 57727992, ; 9: ja\Microsoft.Data.SqlClient.resources => 0x370dbf8 => 308
	i32 66541672, ; 10: System.Diagnostics.StackTrace => 0x3f75868 => 29
	i32 67008169, ; 11: zh-Hant\Microsoft.Maui.Controls.resources => 0x3fe76a9 => 347
	i32 68219467, ; 12: System.Security.Cryptography.Primitives => 0x410f24b => 123
	i32 72070932, ; 13: Microsoft.Maui.Graphics.dll => 0x44bb714 => 203
	i32 82292897, ; 14: System.Runtime.CompilerServices.VisualC.dll => 0x4e7b0a1 => 101
	i32 101534019, ; 15: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 280
	i32 117431740, ; 16: System.Runtime.InteropServices => 0x6ffddbc => 106
	i32 120558881, ; 17: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 280
	i32 122350210, ; 18: System.Threading.Channels.dll => 0x74aea82 => 138
	i32 134690465, ; 19: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x80736a1 => 300
	i32 139659294, ; 20: ja/Microsoft.Data.SqlClient.resources.dll => 0x853081e => 308
	i32 142721839, ; 21: System.Net.WebHeaderCollection => 0x881c32f => 76
	i32 149972175, ; 22: System.Security.Cryptography.Primitives.dll => 0x8f064cf => 123
	i32 159306688, ; 23: System.ComponentModel.Annotations => 0x97ed3c0 => 13
	i32 165246403, ; 24: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 236
	i32 166535111, ; 25: ru/Microsoft.Data.SqlClient.resources.dll => 0x9ed1fc7 => 311
	i32 176265551, ; 26: System.ServiceProcess => 0xa81994f => 131
	i32 182336117, ; 27: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 282
	i32 184328833, ; 28: System.ValueTuple.dll => 0xafca281 => 150
	i32 195452805, ; 29: vi/Microsoft.Maui.Controls.resources.dll => 0xba65f85 => 344
	i32 199333315, ; 30: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xbe195c3 => 345
	i32 205061960, ; 31: System.ComponentModel => 0xc38ff48 => 18
	i32 209399409, ; 32: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 234
	i32 220171995, ; 33: System.Diagnostics.Debug => 0xd1f8edb => 26
	i32 230216969, ; 34: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 256
	i32 230752869, ; 35: Microsoft.CSharp.dll => 0xdc10265 => 1
	i32 231409092, ; 36: System.Linq.Parallel => 0xdcb05c4 => 58
	i32 231814094, ; 37: System.Globalization => 0xdd133ce => 41
	i32 246610117, ; 38: System.Reflection.Emit.Lightweight => 0xeb2f8c5 => 90
	i32 261689757, ; 39: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 239
	i32 264223668, ; 40: zh-Hans\Microsoft.Data.SqlClient.resources => 0xfbfbbb4 => 312
	i32 276479776, ; 41: System.Threading.Timer.dll => 0x107abf20 => 146
	i32 278686392, ; 42: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 258
	i32 280482487, ; 43: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 255
	i32 280992041, ; 44: cs/Microsoft.Maui.Controls.resources.dll => 0x10bf9929 => 316
	i32 291076382, ; 45: System.IO.Pipes.AccessControl.dll => 0x1159791e => 53
	i32 298918909, ; 46: System.Net.Ping.dll => 0x11d123fd => 68
	i32 317674968, ; 47: vi\Microsoft.Maui.Controls.resources => 0x12ef55d8 => 344
	i32 318968648, ; 48: Xamarin.AndroidX.Activity.dll => 0x13031348 => 225
	i32 321597661, ; 49: System.Numerics => 0x132b30dd => 82
	i32 330147069, ; 50: Microsoft.SqlServer.Server => 0x13ada4fd => 204
	i32 336156722, ; 51: ja/Microsoft.Maui.Controls.resources.dll => 0x14095832 => 329
	i32 342366114, ; 52: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 257
	i32 356389973, ; 53: it/Microsoft.Maui.Controls.resources.dll => 0x153e1455 => 328
	i32 360082299, ; 54: System.ServiceModel.Web => 0x15766b7b => 130
	i32 364942007, ; 55: SkiaSharp.Extended.UI => 0x15c092b7 => 207
	i32 367780167, ; 56: System.IO.Pipes => 0x15ebe147 => 54
	i32 374914964, ; 57: System.Transactions.Local => 0x1658bf94 => 148
	i32 375677976, ; 58: System.Net.ServicePoint.dll => 0x16646418 => 73
	i32 379916513, ; 59: System.Threading.Thread.dll => 0x16a510e1 => 144
	i32 382590210, ; 60: SkiaSharp.Extended.dll => 0x16cddd02 => 206
	i32 385762202, ; 61: System.Memory.dll => 0x16fe439a => 61
	i32 392610295, ; 62: System.Threading.ThreadPool.dll => 0x1766c1f7 => 145
	i32 395744057, ; 63: _Microsoft.Android.Resource.Designer => 0x17969339 => 348
	i32 403441872, ; 64: WindowsBase => 0x180c08d0 => 164
	i32 435591531, ; 65: sv/Microsoft.Maui.Controls.resources.dll => 0x19f6996b => 340
	i32 441335492, ; 66: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 240
	i32 442565967, ; 67: System.Collections => 0x1a61054f => 12
	i32 450948140, ; 68: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 253
	i32 451504562, ; 69: System.Security.Cryptography.X509Certificates => 0x1ae969b2 => 124
	i32 456227837, ; 70: System.Web.HttpUtility.dll => 0x1b317bfd => 151
	i32 459347974, ; 71: System.Runtime.Serialization.Primitives.dll => 0x1b611806 => 112
	i32 465846621, ; 72: mscorlib => 0x1bc4415d => 165
	i32 469710990, ; 73: System.dll => 0x1bff388e => 163
	i32 476646585, ; 74: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 255
	i32 485463106, ; 75: Microsoft.IdentityModel.Abstractions => 0x1cef9442 => 192
	i32 486930444, ; 76: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 268
	i32 498788369, ; 77: System.ObjectModel => 0x1dbae811 => 83
	i32 500358224, ; 78: id/Microsoft.Maui.Controls.resources.dll => 0x1dd2dc50 => 327
	i32 503918385, ; 79: fi/Microsoft.Maui.Controls.resources.dll => 0x1e092f31 => 321
	i32 504833739, ; 80: SkiaSharp.SceneGraph => 0x1e1726cb => 208
	i32 513247710, ; 81: Microsoft.Extensions.Primitives.dll => 0x1e9789de => 189
	i32 525008092, ; 82: SkiaSharp.dll => 0x1f4afcdc => 205
	i32 526420162, ; 83: System.Transactions.dll => 0x1f6088c2 => 149
	i32 527452488, ; 84: Xamarin.Kotlin.StdLib.Jdk7 => 0x1f704948 => 300
	i32 530272170, ; 85: System.Linq.Queryable => 0x1f9b4faa => 59
	i32 539058512, ; 86: Microsoft.Extensions.Logging => 0x20216150 => 185
	i32 540030774, ; 87: System.IO.FileSystem.dll => 0x20303736 => 50
	i32 545304856, ; 88: System.Runtime.Extensions => 0x2080b118 => 102
	i32 546455878, ; 89: System.Runtime.Serialization.Xml => 0x20924146 => 113
	i32 548916678, ; 90: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 179
	i32 549171840, ; 91: System.Globalization.Calendars => 0x20bbb280 => 39
	i32 557405415, ; 92: Jsr305Binding => 0x213954e7 => 293
	i32 569601784, ; 93: Xamarin.AndroidX.Window.Extensions.Core.Core => 0x21f36ef8 => 291
	i32 577335427, ; 94: System.Security.Cryptography.Cng => 0x22697083 => 119
	i32 592146354, ; 95: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x234b6fb2 => 335
	i32 597488923, ; 96: CommunityToolkit.Maui => 0x239cf51b => 174
	i32 601371474, ; 97: System.IO.IsolatedStorage.dll => 0x23d83352 => 51
	i32 605376203, ; 98: System.IO.Compression.FileSystem => 0x24154ecb => 43
	i32 613668793, ; 99: System.Security.Cryptography.Algorithms => 0x2493d7b9 => 118
	i32 627609679, ; 100: Xamarin.AndroidX.CustomView => 0x2568904f => 245
	i32 627931235, ; 101: nl\Microsoft.Maui.Controls.resources => 0x256d7863 => 333
	i32 639843206, ; 102: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x26233b86 => 251
	i32 643868501, ; 103: System.Net => 0x2660a755 => 80
	i32 662205335, ; 104: System.Text.Encodings.Web.dll => 0x27787397 => 135
	i32 663517072, ; 105: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 287
	i32 666292255, ; 106: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 232
	i32 672442732, ; 107: System.Collections.Concurrent => 0x2814a96c => 8
	i32 683518922, ; 108: System.Net.Security => 0x28bdabca => 72
	i32 688181140, ; 109: ca/Microsoft.Maui.Controls.resources.dll => 0x2904cf94 => 315
	i32 690569205, ; 110: System.Xml.Linq.dll => 0x29293ff5 => 154
	i32 690602616, ; 111: SkiaSharp.Skottie.dll => 0x2929c278 => 209
	i32 691348768, ; 112: Xamarin.KotlinX.Coroutines.Android.dll => 0x29352520 => 302
	i32 693804605, ; 113: System.Windows => 0x295a9e3d => 153
	i32 699345723, ; 114: System.Reflection.Emit => 0x29af2b3b => 91
	i32 700284507, ; 115: Xamarin.Jetbrains.Annotations => 0x29bd7e5b => 297
	i32 700358131, ; 116: System.IO.Compression.ZipFile => 0x29be9df3 => 44
	i32 706645707, ; 117: ko/Microsoft.Maui.Controls.resources.dll => 0x2a1e8ecb => 330
	i32 709557578, ; 118: de/Microsoft.Maui.Controls.resources.dll => 0x2a4afd4a => 318
	i32 720511267, ; 119: Xamarin.Kotlin.StdLib.Jdk8 => 0x2af22123 => 301
	i32 722857257, ; 120: System.Runtime.Loader.dll => 0x2b15ed29 => 108
	i32 723796036, ; 121: System.ClientModel.dll => 0x2b244044 => 213
	i32 735137430, ; 122: System.Security.SecureString.dll => 0x2bd14e96 => 128
	i32 738469988, ; 123: SkiaSharp.SceneGraph.dll => 0x2c042864 => 208
	i32 752232764, ; 124: System.Diagnostics.Contracts.dll => 0x2cd6293c => 25
	i32 755313932, ; 125: Xamarin.Android.Glide.Annotations.dll => 0x2d052d0c => 222
	i32 759454413, ; 126: System.Net.Requests => 0x2d445acd => 71
	i32 762598435, ; 127: System.IO.Pipes.dll => 0x2d745423 => 54
	i32 775507847, ; 128: System.IO.Compression => 0x2e394f87 => 45
	i32 777317022, ; 129: sk\Microsoft.Maui.Controls.resources => 0x2e54ea9e => 339
	i32 778804420, ; 130: SkiaSharp.Extended.UI.dll => 0x2e6b9cc4 => 207
	i32 789151979, ; 131: Microsoft.Extensions.Options => 0x2f0980eb => 188
	i32 790371945, ; 132: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 0x2f1c1e69 => 246
	i32 797429506, ; 133: AppTransporte.dll => 0x2f87cf02 => 0
	i32 804715423, ; 134: System.Data.Common => 0x2ff6fb9f => 22
	i32 807930345, ; 135: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 0x302809e9 => 260
	i32 823281589, ; 136: System.Private.Uri.dll => 0x311247b5 => 85
	i32 830298997, ; 137: System.IO.Compression.Brotli => 0x317d5b75 => 42
	i32 832635846, ; 138: System.Xml.XPath.dll => 0x31a103c6 => 159
	i32 834051424, ; 139: System.Net.Quic => 0x31b69d60 => 70
	i32 843511501, ; 140: Xamarin.AndroidX.Print => 0x3246f6cd => 273
	i32 873119928, ; 141: Microsoft.VisualBasic => 0x340ac0b8 => 3
	i32 877678880, ; 142: System.Globalization.dll => 0x34505120 => 41
	i32 878954865, ; 143: System.Net.Http.Json => 0x3463c971 => 62
	i32 904024072, ; 144: System.ComponentModel.Primitives.dll => 0x35e25008 => 16
	i32 911108515, ; 145: System.IO.MemoryMappedFiles.dll => 0x364e69a3 => 52
	i32 926902833, ; 146: tr/Microsoft.Maui.Controls.resources.dll => 0x373f6a31 => 342
	i32 928116545, ; 147: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 296
	i32 952186615, ; 148: System.Runtime.InteropServices.JavaScript.dll => 0x38c136f7 => 104
	i32 956575887, ; 149: Xamarin.Kotlin.StdLib.Jdk8.dll => 0x3904308f => 301
	i32 966729478, ; 150: Xamarin.Google.Crypto.Tink.Android => 0x399f1f06 => 294
	i32 967690846, ; 151: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 257
	i32 975236339, ; 152: System.Diagnostics.Tracing => 0x3a20ecf3 => 33
	i32 975874589, ; 153: System.Xml.XDocument => 0x3a2aaa1d => 157
	i32 986514023, ; 154: System.Private.DataContractSerialization.dll => 0x3acd0267 => 84
	i32 987214855, ; 155: System.Diagnostics.Tools => 0x3ad7b407 => 31
	i32 992768348, ; 156: System.Collections.dll => 0x3b2c715c => 12
	i32 994442037, ; 157: System.IO.FileSystem => 0x3b45fb35 => 50
	i32 1001831731, ; 158: System.IO.UnmanagedMemoryStream.dll => 0x3bb6bd33 => 55
	i32 1012816738, ; 159: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 277
	i32 1019214401, ; 160: System.Drawing => 0x3cbffa41 => 35
	i32 1028951442, ; 161: Microsoft.Extensions.DependencyInjection.Abstractions => 0x3d548d92 => 184
	i32 1029334545, ; 162: da/Microsoft.Maui.Controls.resources.dll => 0x3d5a6611 => 317
	i32 1031528504, ; 163: Xamarin.Google.ErrorProne.Annotations.dll => 0x3d7be038 => 295
	i32 1035644815, ; 164: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 230
	i32 1036536393, ; 165: System.Drawing.Primitives.dll => 0x3dc84a49 => 34
	i32 1044663988, ; 166: System.Linq.Expressions.dll => 0x3e444eb4 => 57
	i32 1048439329, ; 167: de/Microsoft.Data.SqlClient.resources.dll => 0x3e7dea21 => 304
	i32 1052210849, ; 168: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 264
	i32 1062017875, ; 169: Microsoft.Identity.Client.Extensions.Msal => 0x3f4d1b53 => 191
	i32 1067306892, ; 170: GoogleGson => 0x3f9dcf8c => 178
	i32 1082857460, ; 171: System.ComponentModel.TypeConverter => 0x408b17f4 => 17
	i32 1084122840, ; 172: Xamarin.Kotlin.StdLib => 0x409e66d8 => 298
	i32 1089913930, ; 173: System.Diagnostics.EventLog.dll => 0x40f6c44a => 216
	i32 1098259244, ; 174: System => 0x41761b2c => 163
	i32 1118262833, ; 175: ko\Microsoft.Maui.Controls.resources => 0x42a75631 => 330
	i32 1121599056, ; 176: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 0x42da3e50 => 263
	i32 1127624469, ; 177: Microsoft.Extensions.Logging.Debug => 0x43362f15 => 187
	i32 1138436374, ; 178: Microsoft.Data.SqlClient.dll => 0x43db2916 => 180
	i32 1149092582, ; 179: Xamarin.AndroidX.Window => 0x447dc2e6 => 290
	i32 1168523401, ; 180: pt\Microsoft.Maui.Controls.resources => 0x45a64089 => 336
	i32 1170634674, ; 181: System.Web.dll => 0x45c677b2 => 152
	i32 1175144683, ; 182: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 286
	i32 1178241025, ; 183: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 271
	i32 1203215381, ; 184: pl/Microsoft.Maui.Controls.resources.dll => 0x47b79c15 => 334
	i32 1204270330, ; 185: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 232
	i32 1208641965, ; 186: System.Diagnostics.Process => 0x480a69ad => 28
	i32 1214827643, ; 187: CommunityToolkit.Mvvm => 0x4868cc7b => 176
	i32 1219128291, ; 188: System.IO.IsolatedStorage => 0x48aa6be3 => 51
	i32 1234928153, ; 189: nb/Microsoft.Maui.Controls.resources.dll => 0x499b8219 => 332
	i32 1243150071, ; 190: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 0x4a18f6f7 => 291
	i32 1253011324, ; 191: Microsoft.Win32.Registry => 0x4aaf6f7c => 5
	i32 1260983243, ; 192: cs\Microsoft.Maui.Controls.resources => 0x4b2913cb => 316
	i32 1264511973, ; 193: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0x4b5eebe5 => 281
	i32 1267360935, ; 194: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 285
	i32 1273260888, ; 195: Xamarin.AndroidX.Collection.Ktx => 0x4be46b58 => 237
	i32 1275534314, ; 196: Xamarin.KotlinX.Coroutines.Android => 0x4c071bea => 302
	i32 1278448581, ; 197: Xamarin.AndroidX.Annotation.Jvm => 0x4c3393c5 => 229
	i32 1293217323, ; 198: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 248
	i32 1309188875, ; 199: System.Private.DataContractSerialization => 0x4e08a30b => 84
	i32 1322716291, ; 200: Xamarin.AndroidX.Window.dll => 0x4ed70c83 => 290
	i32 1324164729, ; 201: System.Linq => 0x4eed2679 => 60
	i32 1335329327, ; 202: System.Runtime.Serialization.Json.dll => 0x4f97822f => 111
	i32 1364015309, ; 203: System.IO => 0x514d38cd => 56
	i32 1373134921, ; 204: zh-Hans\Microsoft.Maui.Controls.resources => 0x51d86049 => 346
	i32 1376866003, ; 205: Xamarin.AndroidX.SavedState => 0x52114ed3 => 277
	i32 1379779777, ; 206: System.Resources.ResourceManager => 0x523dc4c1 => 98
	i32 1402170036, ; 207: System.Configuration.dll => 0x53936ab4 => 19
	i32 1406073936, ; 208: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 241
	i32 1408764838, ; 209: System.Runtime.Serialization.Formatters.dll => 0x53f80ba6 => 110
	i32 1411638395, ; 210: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 100
	i32 1422545099, ; 211: System.Runtime.CompilerServices.VisualC => 0x54ca50cb => 101
	i32 1430672901, ; 212: ar\Microsoft.Maui.Controls.resources => 0x55465605 => 314
	i32 1434145427, ; 213: System.Runtime.Handles => 0x557b5293 => 103
	i32 1435222561, ; 214: Xamarin.Google.Crypto.Tink.Android.dll => 0x558bc221 => 294
	i32 1439761251, ; 215: System.Net.Quic.dll => 0x55d10363 => 70
	i32 1452070440, ; 216: System.Formats.Asn1.dll => 0x568cd628 => 37
	i32 1453312822, ; 217: System.Diagnostics.Tools.dll => 0x569fcb36 => 31
	i32 1457743152, ; 218: System.Runtime.Extensions.dll => 0x56e36530 => 102
	i32 1458022317, ; 219: System.Net.Security.dll => 0x56e7a7ad => 72
	i32 1460893475, ; 220: System.IdentityModel.Tokens.Jwt => 0x57137723 => 217
	i32 1461004990, ; 221: es\Microsoft.Maui.Controls.resources => 0x57152abe => 320
	i32 1461234159, ; 222: System.Collections.Immutable.dll => 0x5718a9ef => 9
	i32 1461719063, ; 223: System.Security.Cryptography.OpenSsl => 0x57201017 => 122
	i32 1462112819, ; 224: System.IO.Compression.dll => 0x57261233 => 45
	i32 1469204771, ; 225: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 231
	i32 1470490898, ; 226: Microsoft.Extensions.Primitives => 0x57a5e912 => 189
	i32 1479771757, ; 227: System.Collections.Immutable => 0x5833866d => 9
	i32 1480492111, ; 228: System.IO.Compression.Brotli.dll => 0x583e844f => 42
	i32 1487239319, ; 229: Microsoft.Win32.Primitives => 0x58a57897 => 4
	i32 1490025113, ; 230: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 0x58cffa99 => 278
	i32 1493001747, ; 231: hi/Microsoft.Maui.Controls.resources.dll => 0x58fd6613 => 324
	i32 1498168481, ; 232: Microsoft.IdentityModel.JsonWebTokens.dll => 0x594c3ca1 => 193
	i32 1514721132, ; 233: el/Microsoft.Maui.Controls.resources.dll => 0x5a48cf6c => 319
	i32 1536373174, ; 234: System.Diagnostics.TextWriterTraceListener => 0x5b9331b6 => 30
	i32 1543031311, ; 235: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 137
	i32 1543355203, ; 236: System.Reflection.Emit.dll => 0x5bfdbb43 => 91
	i32 1550322496, ; 237: System.Reflection.Extensions.dll => 0x5c680b40 => 92
	i32 1551623176, ; 238: sk/Microsoft.Maui.Controls.resources.dll => 0x5c7be408 => 339
	i32 1565310744, ; 239: System.Runtime.Caching => 0x5d4cbf18 => 219
	i32 1565862583, ; 240: System.IO.FileSystem.Primitives => 0x5d552ab7 => 48
	i32 1566207040, ; 241: System.Threading.Tasks.Dataflow.dll => 0x5d5a6c40 => 140
	i32 1573704789, ; 242: System.Runtime.Serialization.Json => 0x5dccd455 => 111
	i32 1580037396, ; 243: System.Threading.Overlapped => 0x5e2d7514 => 139
	i32 1582305585, ; 244: Azure.Identity => 0x5e501131 => 173
	i32 1582372066, ; 245: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 247
	i32 1592978981, ; 246: System.Runtime.Serialization.dll => 0x5ef2ee25 => 114
	i32 1596263029, ; 247: zh-Hant\Microsoft.Data.SqlClient.resources => 0x5f250a75 => 313
	i32 1597949149, ; 248: Xamarin.Google.ErrorProne.Annotations => 0x5f3ec4dd => 295
	i32 1601112923, ; 249: System.Xml.Serialization => 0x5f6f0b5b => 156
	i32 1604827217, ; 250: System.Net.WebClient => 0x5fa7b851 => 75
	i32 1618516317, ; 251: System.Net.WebSockets.Client.dll => 0x6078995d => 78
	i32 1622152042, ; 252: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 267
	i32 1622358360, ; 253: System.Dynamic.Runtime => 0x60b33958 => 36
	i32 1623212457, ; 254: SkiaSharp.Views.Maui.Controls => 0x60c041a9 => 211
	i32 1624863272, ; 255: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 289
	i32 1628113371, ; 256: Microsoft.IdentityModel.Protocols.OpenIdConnect => 0x610b09db => 196
	i32 1634654947, ; 257: CommunityToolkit.Maui.Core.dll => 0x616edae3 => 175
	i32 1635184631, ; 258: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x6176eff7 => 251
	i32 1636350590, ; 259: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 244
	i32 1639515021, ; 260: System.Net.Http.dll => 0x61b9038d => 63
	i32 1639986890, ; 261: System.Text.RegularExpressions => 0x61c036ca => 137
	i32 1641389582, ; 262: System.ComponentModel.EventBasedAsync.dll => 0x61d59e0e => 15
	i32 1657153582, ; 263: System.Runtime => 0x62c6282e => 115
	i32 1658241508, ; 264: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 283
	i32 1658251792, ; 265: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 292
	i32 1670060433, ; 266: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 239
	i32 1675553242, ; 267: System.IO.FileSystem.DriveInfo.dll => 0x63dee9da => 47
	i32 1677501392, ; 268: System.Net.Primitives.dll => 0x63fca3d0 => 69
	i32 1678508291, ; 269: System.Net.WebSockets => 0x640c0103 => 79
	i32 1679769178, ; 270: System.Security.Cryptography => 0x641f3e5a => 125
	i32 1691477237, ; 271: System.Reflection.Metadata => 0x64d1e4f5 => 93
	i32 1696967625, ; 272: System.Security.Cryptography.Csp => 0x6525abc9 => 120
	i32 1698840827, ; 273: Xamarin.Kotlin.StdLib.Common => 0x654240fb => 299
	i32 1701541528, ; 274: System.Diagnostics.Debug.dll => 0x656b7698 => 26
	i32 1720223769, ; 275: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 0x66888819 => 260
	i32 1724472758, ; 276: SkiaSharp.Extended => 0x66c95db6 => 206
	i32 1726116996, ; 277: System.Reflection.dll => 0x66e27484 => 96
	i32 1728033016, ; 278: System.Diagnostics.FileVersionInfo.dll => 0x66ffb0f8 => 27
	i32 1729485958, ; 279: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 235
	i32 1736233607, ; 280: ro/Microsoft.Maui.Controls.resources.dll => 0x677cd287 => 337
	i32 1743415430, ; 281: ca\Microsoft.Maui.Controls.resources => 0x67ea6886 => 315
	i32 1744735666, ; 282: System.Transactions.Local.dll => 0x67fe8db2 => 148
	i32 1746316138, ; 283: Mono.Android.Export => 0x6816ab6a => 168
	i32 1750313021, ; 284: Microsoft.Win32.Primitives.dll => 0x6853a83d => 4
	i32 1758240030, ; 285: System.Resources.Reader.dll => 0x68cc9d1e => 97
	i32 1763938596, ; 286: System.Diagnostics.TraceSource.dll => 0x69239124 => 32
	i32 1765942094, ; 287: System.Reflection.Extensions => 0x6942234e => 92
	i32 1766324549, ; 288: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 282
	i32 1770582343, ; 289: Microsoft.Extensions.Logging.dll => 0x6988f147 => 185
	i32 1776026572, ; 290: System.Core.dll => 0x69dc03cc => 21
	i32 1777075843, ; 291: System.Globalization.Extensions.dll => 0x69ec0683 => 40
	i32 1780572499, ; 292: Mono.Android.Runtime.dll => 0x6a216153 => 169
	i32 1782862114, ; 293: ms\Microsoft.Maui.Controls.resources => 0x6a445122 => 331
	i32 1788241197, ; 294: Xamarin.AndroidX.Fragment => 0x6a96652d => 253
	i32 1793755602, ; 295: he\Microsoft.Maui.Controls.resources => 0x6aea89d2 => 323
	i32 1794500907, ; 296: Microsoft.Identity.Client.dll => 0x6af5e92b => 190
	i32 1796167890, ; 297: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 179
	i32 1808609942, ; 298: Xamarin.AndroidX.Loader => 0x6bcd3296 => 267
	i32 1813058853, ; 299: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 298
	i32 1813201214, ; 300: Xamarin.Google.Android.Material => 0x6c13413e => 292
	i32 1818569960, ; 301: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 272
	i32 1818787751, ; 302: Microsoft.VisualBasic.Core => 0x6c687fa7 => 2
	i32 1824175904, ; 303: System.Text.Encoding.Extensions => 0x6cbab720 => 133
	i32 1824722060, ; 304: System.Runtime.Serialization.Formatters => 0x6cc30c8c => 110
	i32 1828688058, ; 305: Microsoft.Extensions.Logging.Abstractions.dll => 0x6cff90ba => 186
	i32 1842015223, ; 306: uk/Microsoft.Maui.Controls.resources.dll => 0x6dcaebf7 => 343
	i32 1847515442, ; 307: Xamarin.Android.Glide.Annotations => 0x6e1ed932 => 222
	i32 1853025655, ; 308: sv\Microsoft.Maui.Controls.resources => 0x6e72ed77 => 340
	i32 1858542181, ; 309: System.Linq.Expressions => 0x6ec71a65 => 57
	i32 1870277092, ; 310: System.Reflection.Primitives => 0x6f7a29e4 => 94
	i32 1871986876, ; 311: Microsoft.IdentityModel.Protocols.OpenIdConnect.dll => 0x6f9440bc => 196
	i32 1875935024, ; 312: fr\Microsoft.Maui.Controls.resources => 0x6fd07f30 => 322
	i32 1879696579, ; 313: System.Formats.Tar.dll => 0x7009e4c3 => 38
	i32 1885316902, ; 314: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 233
	i32 1888955245, ; 315: System.Diagnostics.Contracts => 0x70972b6d => 25
	i32 1889954781, ; 316: System.Reflection.Metadata.dll => 0x70a66bdd => 93
	i32 1898237753, ; 317: System.Reflection.DispatchProxy => 0x7124cf39 => 88
	i32 1900610850, ; 318: System.Resources.ResourceManager.dll => 0x71490522 => 98
	i32 1910275211, ; 319: System.Collections.NonGeneric.dll => 0x71dc7c8b => 10
	i32 1939592360, ; 320: System.Private.Xml.Linq => 0x739bd4a8 => 86
	i32 1956758971, ; 321: System.Resources.Writer => 0x74a1c5bb => 99
	i32 1961813231, ; 322: Xamarin.AndroidX.Security.SecurityCrypto.dll => 0x74eee4ef => 279
	i32 1968388702, ; 323: Microsoft.Extensions.Configuration.dll => 0x75533a5e => 181
	i32 1983156543, ; 324: Xamarin.Kotlin.StdLib.Common.dll => 0x7634913f => 299
	i32 1985761444, ; 325: Xamarin.Android.Glide.GifDecoder => 0x765c50a4 => 224
	i32 1986222447, ; 326: Microsoft.IdentityModel.Tokens.dll => 0x7663596f => 197
	i32 2003115576, ; 327: el\Microsoft.Maui.Controls.resources => 0x77651e38 => 319
	i32 2011961780, ; 328: System.Buffers.dll => 0x77ec19b4 => 7
	i32 2019465201, ; 329: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 264
	i32 2025202353, ; 330: ar/Microsoft.Maui.Controls.resources.dll => 0x78b622b1 => 314
	i32 2031763787, ; 331: Xamarin.Android.Glide => 0x791a414b => 221
	i32 2040764568, ; 332: Microsoft.Identity.Client.Extensions.Msal.dll => 0x79a39898 => 191
	i32 2045470958, ; 333: System.Private.Xml => 0x79eb68ee => 87
	i32 2055257422, ; 334: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 259
	i32 2060060697, ; 335: System.Windows.dll => 0x7aca0819 => 153
	i32 2066184531, ; 336: de\Microsoft.Maui.Controls.resources => 0x7b277953 => 318
	i32 2070888862, ; 337: System.Diagnostics.TraceSource => 0x7b6f419e => 32
	i32 2079903147, ; 338: System.Runtime.dll => 0x7bf8cdab => 115
	i32 2090596640, ; 339: System.Numerics.Vectors => 0x7c9bf920 => 81
	i32 2127167465, ; 340: System.Console => 0x7ec9ffe9 => 20
	i32 2142473426, ; 341: System.Collections.Specialized => 0x7fb38cd2 => 11
	i32 2143790110, ; 342: System.Xml.XmlSerializer.dll => 0x7fc7a41e => 161
	i32 2146852085, ; 343: Microsoft.VisualBasic.dll => 0x7ff65cf5 => 3
	i32 2159891885, ; 344: Microsoft.Maui => 0x80bd55ad => 201
	i32 2169148018, ; 345: hu\Microsoft.Maui.Controls.resources => 0x814a9272 => 326
	i32 2181898931, ; 346: Microsoft.Extensions.Options.dll => 0x820d22b3 => 188
	i32 2192057212, ; 347: Microsoft.Extensions.Logging.Abstractions => 0x82a8237c => 186
	i32 2193016926, ; 348: System.ObjectModel.dll => 0x82b6c85e => 83
	i32 2201107256, ; 349: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 303
	i32 2201231467, ; 350: System.Net.Http => 0x8334206b => 63
	i32 2207618523, ; 351: it\Microsoft.Maui.Controls.resources => 0x839595db => 328
	i32 2217644978, ; 352: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 286
	i32 2222056684, ; 353: System.Threading.Tasks.Parallel => 0x8471e4ec => 142
	i32 2228745826, ; 354: pt-BR\Microsoft.Data.SqlClient.resources => 0x84d7f662 => 310
	i32 2244775296, ; 355: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 268
	i32 2252106437, ; 356: System.Xml.Serialization.dll => 0x863c6ac5 => 156
	i32 2253551641, ; 357: Microsoft.IdentityModel.Protocols => 0x86527819 => 195
	i32 2256313426, ; 358: System.Globalization.Extensions => 0x867c9c52 => 40
	i32 2265110946, ; 359: System.Security.AccessControl.dll => 0x8702d9a2 => 116
	i32 2266799131, ; 360: Microsoft.Extensions.Configuration.Abstractions => 0x871c9c1b => 182
	i32 2267999099, ; 361: Xamarin.Android.Glide.DiskLruCache.dll => 0x872eeb7b => 223
	i32 2270573516, ; 362: fr/Microsoft.Maui.Controls.resources.dll => 0x875633cc => 322
	i32 2279755925, ; 363: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 275
	i32 2293034957, ; 364: System.ServiceModel.Web.dll => 0x88acefcd => 130
	i32 2295906218, ; 365: System.Net.Sockets => 0x88d8bfaa => 74
	i32 2298471582, ; 366: System.Net.Mail => 0x88ffe49e => 65
	i32 2303942373, ; 367: nb\Microsoft.Maui.Controls.resources => 0x89535ee5 => 332
	i32 2305521784, ; 368: System.Private.CoreLib.dll => 0x896b7878 => 171
	i32 2309278602, ; 369: ko\Microsoft.Data.SqlClient.resources => 0x89a4cb8a => 309
	i32 2315684594, ; 370: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 227
	i32 2320631194, ; 371: System.Threading.Tasks.Parallel.dll => 0x8a52059a => 142
	i32 2340441535, ; 372: System.Runtime.InteropServices.RuntimeInformation.dll => 0x8b804dbf => 105
	i32 2344264397, ; 373: System.ValueTuple => 0x8bbaa2cd => 150
	i32 2353062107, ; 374: System.Net.Primitives => 0x8c40e0db => 69
	i32 2364201794, ; 375: SkiaSharp.Views.Maui.Core => 0x8ceadb42 => 212
	i32 2368005991, ; 376: System.Xml.ReaderWriter.dll => 0x8d24e767 => 155
	i32 2369706906, ; 377: Microsoft.IdentityModel.Logging => 0x8d3edb9a => 194
	i32 2371007202, ; 378: Microsoft.Extensions.Configuration => 0x8d52b2e2 => 181
	i32 2378619854, ; 379: System.Security.Cryptography.Csp.dll => 0x8dc6dbce => 120
	i32 2383496789, ; 380: System.Security.Principal.Windows.dll => 0x8e114655 => 126
	i32 2395872292, ; 381: id\Microsoft.Maui.Controls.resources => 0x8ece1c24 => 327
	i32 2401565422, ; 382: System.Web.HttpUtility => 0x8f24faee => 151
	i32 2403452196, ; 383: Xamarin.AndroidX.Emoji2.dll => 0x8f41c524 => 250
	i32 2421380589, ; 384: System.Threading.Tasks.Dataflow => 0x905355ed => 140
	i32 2423080555, ; 385: Xamarin.AndroidX.Collection.Ktx.dll => 0x906d466b => 237
	i32 2427813419, ; 386: hi\Microsoft.Maui.Controls.resources => 0x90b57e2b => 324
	i32 2435356389, ; 387: System.Console.dll => 0x912896e5 => 20
	i32 2435904999, ; 388: System.ComponentModel.DataAnnotations.dll => 0x9130f5e7 => 14
	i32 2454642406, ; 389: System.Text.Encoding.dll => 0x924edee6 => 134
	i32 2458678730, ; 390: System.Net.Sockets.dll => 0x928c75ca => 74
	i32 2459001652, ; 391: System.Linq.Parallel.dll => 0x92916334 => 58
	i32 2465532216, ; 392: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 240
	i32 2471841756, ; 393: netstandard.dll => 0x93554fdc => 166
	i32 2475788418, ; 394: Java.Interop.dll => 0x93918882 => 167
	i32 2480646305, ; 395: Microsoft.Maui.Controls => 0x93dba8a1 => 199
	i32 2483903535, ; 396: System.ComponentModel.EventBasedAsync => 0x940d5c2f => 15
	i32 2484371297, ; 397: System.Net.ServicePoint => 0x94147f61 => 73
	i32 2490993605, ; 398: System.AppContext.dll => 0x94798bc5 => 6
	i32 2501346920, ; 399: System.Data.DataSetExtensions => 0x95178668 => 23
	i32 2505896520, ; 400: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 262
	i32 2509217888, ; 401: System.Diagnostics.EventLog => 0x958fa060 => 216
	i32 2522472828, ; 402: Xamarin.Android.Glide.dll => 0x9659e17c => 221
	i32 2538310050, ; 403: System.Reflection.Emit.Lightweight.dll => 0x974b89a2 => 90
	i32 2550873716, ; 404: hr\Microsoft.Maui.Controls.resources => 0x980b3e74 => 325
	i32 2562349572, ; 405: Microsoft.CSharp => 0x98ba5a04 => 1
	i32 2570120770, ; 406: System.Text.Encodings.Web => 0x9930ee42 => 135
	i32 2581783588, ; 407: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 0x99e2e424 => 263
	i32 2581819634, ; 408: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 285
	i32 2585220780, ; 409: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 133
	i32 2585805581, ; 410: System.Net.Ping => 0x9a20430d => 68
	i32 2589602615, ; 411: System.Threading.ThreadPool => 0x9a5a3337 => 145
	i32 2593496499, ; 412: pl\Microsoft.Maui.Controls.resources => 0x9a959db3 => 334
	i32 2605712449, ; 413: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 303
	i32 2615233544, ; 414: Xamarin.AndroidX.Fragment.Ktx => 0x9be14c08 => 254
	i32 2616218305, ; 415: Microsoft.Extensions.Logging.Debug.dll => 0x9bf052c1 => 187
	i32 2617129537, ; 416: System.Private.Xml.dll => 0x9bfe3a41 => 87
	i32 2618712057, ; 417: System.Reflection.TypeExtensions.dll => 0x9c165ff9 => 95
	i32 2620871830, ; 418: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 244
	i32 2624644809, ; 419: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 249
	i32 2625339995, ; 420: SkiaSharp.Views.Maui.Core.dll => 0x9c7b825b => 212
	i32 2626831493, ; 421: ja\Microsoft.Maui.Controls.resources => 0x9c924485 => 329
	i32 2627185994, ; 422: System.Diagnostics.TextWriterTraceListener.dll => 0x9c97ad4a => 30
	i32 2628210652, ; 423: System.Memory.Data => 0x9ca74fdc => 218
	i32 2629843544, ; 424: System.IO.Compression.ZipFile.dll => 0x9cc03a58 => 44
	i32 2633051222, ; 425: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 258
	i32 2640290731, ; 426: Microsoft.IdentityModel.Logging.dll => 0x9d5fa3ab => 194
	i32 2640706905, ; 427: Azure.Core => 0x9d65fd59 => 172
	i32 2660759594, ; 428: System.Security.Cryptography.ProtectedData.dll => 0x9e97f82a => 220
	i32 2663391936, ; 429: Xamarin.Android.Glide.DiskLruCache => 0x9ec022c0 => 223
	i32 2663698177, ; 430: System.Runtime.Loader => 0x9ec4cf01 => 108
	i32 2664396074, ; 431: System.Xml.XDocument.dll => 0x9ecf752a => 157
	i32 2665622720, ; 432: System.Drawing.Primitives => 0x9ee22cc0 => 34
	i32 2676780864, ; 433: System.Data.Common.dll => 0x9f8c6f40 => 22
	i32 2677098746, ; 434: Azure.Identity.dll => 0x9f9148fa => 173
	i32 2686887180, ; 435: System.Runtime.Serialization.Xml.dll => 0xa026a50c => 113
	i32 2693849962, ; 436: System.IO.dll => 0xa090e36a => 56
	i32 2701096212, ; 437: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 283
	i32 2715334215, ; 438: System.Threading.Tasks.dll => 0xa1d8b647 => 143
	i32 2717744543, ; 439: System.Security.Claims => 0xa1fd7d9f => 117
	i32 2719963679, ; 440: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 119
	i32 2724373263, ; 441: System.Runtime.Numerics.dll => 0xa262a30f => 109
	i32 2732626843, ; 442: Xamarin.AndroidX.Activity => 0xa2e0939b => 225
	i32 2735172069, ; 443: System.Threading.Channels => 0xa30769e5 => 138
	i32 2737747696, ; 444: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 231
	i32 2740051746, ; 445: Microsoft.Identity.Client => 0xa351df22 => 190
	i32 2740948882, ; 446: System.IO.Pipes.AccessControl => 0xa35f8f92 => 53
	i32 2748088231, ; 447: System.Runtime.InteropServices.JavaScript => 0xa3cc7fa7 => 104
	i32 2752995522, ; 448: pt-BR\Microsoft.Maui.Controls.resources => 0xa41760c2 => 335
	i32 2755098380, ; 449: Microsoft.SqlServer.Server.dll => 0xa437770c => 204
	i32 2758225723, ; 450: Microsoft.Maui.Controls.Xaml => 0xa4672f3b => 200
	i32 2764765095, ; 451: Microsoft.Maui.dll => 0xa4caf7a7 => 201
	i32 2765824710, ; 452: System.Text.Encoding.CodePages.dll => 0xa4db22c6 => 132
	i32 2770495804, ; 453: Xamarin.Jetbrains.Annotations.dll => 0xa522693c => 297
	i32 2778768386, ; 454: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 288
	i32 2779977773, ; 455: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0xa5b3182d => 276
	i32 2785988530, ; 456: th\Microsoft.Maui.Controls.resources => 0xa60ecfb2 => 341
	i32 2788224221, ; 457: Xamarin.AndroidX.Fragment.Ktx.dll => 0xa630ecdd => 254
	i32 2795602088, ; 458: SkiaSharp.Views.Android.dll => 0xa6a180a8 => 210
	i32 2801831435, ; 459: Microsoft.Maui.Graphics => 0xa7008e0b => 203
	i32 2803228030, ; 460: System.Xml.XPath.XDocument.dll => 0xa715dd7e => 158
	i32 2804509662, ; 461: es/Microsoft.Data.SqlClient.resources.dll => 0xa7296bde => 305
	i32 2806116107, ; 462: es/Microsoft.Maui.Controls.resources.dll => 0xa741ef0b => 320
	i32 2810250172, ; 463: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 241
	i32 2819470561, ; 464: System.Xml.dll => 0xa80db4e1 => 162
	i32 2821205001, ; 465: System.ServiceProcess.dll => 0xa8282c09 => 131
	i32 2821294376, ; 466: Xamarin.AndroidX.ResourceInspection.Annotation => 0xa8298928 => 276
	i32 2824502124, ; 467: System.Xml.XmlDocument => 0xa85a7b6c => 160
	i32 2831556043, ; 468: nl/Microsoft.Maui.Controls.resources.dll => 0xa8c61dcb => 333
	i32 2838993487, ; 469: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 0xa9379a4f => 265
	i32 2841937114, ; 470: it/Microsoft.Data.SqlClient.resources.dll => 0xa96484da => 307
	i32 2849599387, ; 471: System.Threading.Overlapped.dll => 0xa9d96f9b => 139
	i32 2853208004, ; 472: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 288
	i32 2855708567, ; 473: Xamarin.AndroidX.Transition => 0xaa36a797 => 284
	i32 2861098320, ; 474: Mono.Android.Export.dll => 0xaa88e550 => 168
	i32 2861189240, ; 475: Microsoft.Maui.Essentials => 0xaa8a4878 => 202
	i32 2867946736, ; 476: System.Security.Cryptography.ProtectedData => 0xaaf164f0 => 220
	i32 2868488919, ; 477: CommunityToolkit.Maui.Core => 0xaaf9aad7 => 175
	i32 2870099610, ; 478: Xamarin.AndroidX.Activity.Ktx.dll => 0xab123e9a => 226
	i32 2875164099, ; 479: Jsr305Binding.dll => 0xab5f85c3 => 293
	i32 2875220617, ; 480: System.Globalization.Calendars.dll => 0xab606289 => 39
	i32 2884993177, ; 481: Xamarin.AndroidX.ExifInterface => 0xabf58099 => 252
	i32 2887636118, ; 482: System.Net.dll => 0xac1dd496 => 80
	i32 2899753641, ; 483: System.IO.UnmanagedMemoryStream => 0xacd6baa9 => 55
	i32 2900621748, ; 484: System.Dynamic.Runtime.dll => 0xace3f9b4 => 36
	i32 2901442782, ; 485: System.Reflection => 0xacf080de => 96
	i32 2905242038, ; 486: mscorlib.dll => 0xad2a79b6 => 165
	i32 2909740682, ; 487: System.Private.CoreLib => 0xad6f1e8a => 171
	i32 2912489636, ; 488: SkiaSharp.Views.Android => 0xad9910a4 => 210
	i32 2916838712, ; 489: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 289
	i32 2919462931, ; 490: System.Numerics.Vectors.dll => 0xae037813 => 81
	i32 2921128767, ; 491: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 228
	i32 2936416060, ; 492: System.Resources.Reader => 0xaf06273c => 97
	i32 2940926066, ; 493: System.Diagnostics.StackTrace.dll => 0xaf4af872 => 29
	i32 2942453041, ; 494: System.Xml.XPath.XDocument => 0xaf624531 => 158
	i32 2944313911, ; 495: System.Configuration.ConfigurationManager.dll => 0xaf7eaa37 => 214
	i32 2957275192, ; 496: Dapper => 0xb0447038 => 177
	i32 2959614098, ; 497: System.ComponentModel.dll => 0xb0682092 => 18
	i32 2968338931, ; 498: System.Security.Principal.Windows => 0xb0ed41f3 => 126
	i32 2972252294, ; 499: System.Security.Cryptography.Algorithms.dll => 0xb128f886 => 118
	i32 2978675010, ; 500: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 248
	i32 2987532451, ; 501: Xamarin.AndroidX.Security.SecurityCrypto => 0xb21220a3 => 279
	i32 2996846495, ; 502: Xamarin.AndroidX.Lifecycle.Process.dll => 0xb2a03f9f => 261
	i32 3012788804, ; 503: System.Configuration.ConfigurationManager => 0xb3938244 => 214
	i32 3016983068, ; 504: Xamarin.AndroidX.Startup.StartupRuntime => 0xb3d3821c => 281
	i32 3023353419, ; 505: WindowsBase.dll => 0xb434b64b => 164
	i32 3023511517, ; 506: ru\Microsoft.Data.SqlClient.resources => 0xb4371fdd => 311
	i32 3024354802, ; 507: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 256
	i32 3033605958, ; 508: System.Memory.Data.dll => 0xb4d12746 => 218
	i32 3038032645, ; 509: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 348
	i32 3056245963, ; 510: Xamarin.AndroidX.SavedState.SavedState.Ktx => 0xb62a9ccb => 278
	i32 3057625584, ; 511: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 269
	i32 3059408633, ; 512: Mono.Android.Runtime => 0xb65adef9 => 169
	i32 3059793426, ; 513: System.ComponentModel.Primitives => 0xb660be12 => 16
	i32 3075834255, ; 514: System.Threading.Tasks => 0xb755818f => 143
	i32 3077302341, ; 515: hu/Microsoft.Maui.Controls.resources.dll => 0xb76be845 => 326
	i32 3084678329, ; 516: Microsoft.IdentityModel.Tokens => 0xb7dc74b9 => 197
	i32 3090735792, ; 517: System.Security.Cryptography.X509Certificates.dll => 0xb838e2b0 => 124
	i32 3099732863, ; 518: System.Security.Claims.dll => 0xb8c22b7f => 117
	i32 3103600923, ; 519: System.Formats.Asn1 => 0xb8fd311b => 37
	i32 3111772706, ; 520: System.Runtime.Serialization => 0xb979e222 => 114
	i32 3121463068, ; 521: System.IO.FileSystem.AccessControl.dll => 0xba0dbf1c => 46
	i32 3124832203, ; 522: System.Threading.Tasks.Extensions => 0xba4127cb => 141
	i32 3132293585, ; 523: System.Security.AccessControl => 0xbab301d1 => 116
	i32 3147165239, ; 524: System.Diagnostics.Tracing.dll => 0xbb95ee37 => 33
	i32 3148237826, ; 525: GoogleGson.dll => 0xbba64c02 => 178
	i32 3158628304, ; 526: zh-Hant/Microsoft.Data.SqlClient.resources.dll => 0xbc44d7d0 => 313
	i32 3159123045, ; 527: System.Reflection.Primitives.dll => 0xbc4c6465 => 94
	i32 3160747431, ; 528: System.IO.MemoryMappedFiles => 0xbc652da7 => 52
	i32 3178803400, ; 529: Xamarin.AndroidX.Navigation.Fragment.dll => 0xbd78b0c8 => 270
	i32 3192346100, ; 530: System.Security.SecureString => 0xbe4755f4 => 128
	i32 3193515020, ; 531: System.Web => 0xbe592c0c => 152
	i32 3204380047, ; 532: System.Data.dll => 0xbefef58f => 24
	i32 3209718065, ; 533: System.Xml.XmlDocument.dll => 0xbf506931 => 160
	i32 3211777861, ; 534: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 247
	i32 3220365878, ; 535: System.Threading => 0xbff2e236 => 147
	i32 3226221578, ; 536: System.Runtime.Handles.dll => 0xc04c3c0a => 103
	i32 3251039220, ; 537: System.Reflection.DispatchProxy.dll => 0xc1c6ebf4 => 88
	i32 3258312781, ; 538: Xamarin.AndroidX.CardView => 0xc235e84d => 235
	i32 3265493905, ; 539: System.Linq.Queryable.dll => 0xc2a37b91 => 59
	i32 3265893370, ; 540: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 141
	i32 3268887220, ; 541: fr/Microsoft.Data.SqlClient.resources.dll => 0xc2d742b4 => 306
	i32 3276600297, ; 542: pt-BR/Microsoft.Data.SqlClient.resources.dll => 0xc34cf3e9 => 310
	i32 3277815716, ; 543: System.Resources.Writer.dll => 0xc35f7fa4 => 99
	i32 3279906254, ; 544: Microsoft.Win32.Registry.dll => 0xc37f65ce => 5
	i32 3280506390, ; 545: System.ComponentModel.Annotations.dll => 0xc3888e16 => 13
	i32 3290767353, ; 546: System.Security.Cryptography.Encoding => 0xc4251ff9 => 121
	i32 3299363146, ; 547: System.Text.Encoding => 0xc4a8494a => 134
	i32 3303498502, ; 548: System.Diagnostics.FileVersionInfo => 0xc4e76306 => 27
	i32 3305363605, ; 549: fi\Microsoft.Maui.Controls.resources => 0xc503d895 => 321
	i32 3312457198, ; 550: Microsoft.IdentityModel.JsonWebTokens => 0xc57015ee => 193
	i32 3316684772, ; 551: System.Net.Requests.dll => 0xc5b097e4 => 71
	i32 3317135071, ; 552: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 245
	i32 3317144872, ; 553: System.Data => 0xc5b79d28 => 24
	i32 3340387945, ; 554: SkiaSharp => 0xc71a4669 => 205
	i32 3340431453, ; 555: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 233
	i32 3343947874, ; 556: fr\Microsoft.Data.SqlClient.resources => 0xc7509862 => 306
	i32 3345895724, ; 557: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xc76e512c => 274
	i32 3346324047, ; 558: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 271
	i32 3357674450, ; 559: ru\Microsoft.Maui.Controls.resources => 0xc8220bd2 => 338
	i32 3358260929, ; 560: System.Text.Json => 0xc82afec1 => 136
	i32 3362336904, ; 561: Xamarin.AndroidX.Activity.Ktx => 0xc8693088 => 226
	i32 3362522851, ; 562: Xamarin.AndroidX.Core => 0xc86c06e3 => 242
	i32 3366347497, ; 563: Java.Interop => 0xc8a662e9 => 167
	i32 3374879918, ; 564: Microsoft.IdentityModel.Protocols.dll => 0xc92894ae => 195
	i32 3374999561, ; 565: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 275
	i32 3381016424, ; 566: da\Microsoft.Maui.Controls.resources => 0xc9863768 => 317
	i32 3395150330, ; 567: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 100
	i32 3403906625, ; 568: System.Security.Cryptography.OpenSsl.dll => 0xcae37e41 => 122
	i32 3405233483, ; 569: Xamarin.AndroidX.CustomView.PoolingContainer => 0xcaf7bd4b => 246
	i32 3428513518, ; 570: Microsoft.Extensions.DependencyInjection.dll => 0xcc5af6ee => 183
	i32 3429136800, ; 571: System.Xml => 0xcc6479a0 => 162
	i32 3430777524, ; 572: netstandard => 0xcc7d82b4 => 166
	i32 3441283291, ; 573: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 249
	i32 3445260447, ; 574: System.Formats.Tar => 0xcd5a809f => 38
	i32 3452344032, ; 575: Microsoft.Maui.Controls.Compatibility.dll => 0xcdc696e0 => 198
	i32 3463511458, ; 576: hr/Microsoft.Maui.Controls.resources.dll => 0xce70fda2 => 325
	i32 3471940407, ; 577: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 17
	i32 3473156932, ; 578: SkiaSharp.Views.Maui.Controls.dll => 0xcf042b44 => 211
	i32 3476120550, ; 579: Mono.Android => 0xcf3163e6 => 170
	i32 3479583265, ; 580: ru/Microsoft.Maui.Controls.resources.dll => 0xcf663a21 => 338
	i32 3484440000, ; 581: ro\Microsoft.Maui.Controls.resources => 0xcfb055c0 => 337
	i32 3485117614, ; 582: System.Text.Json.dll => 0xcfbaacae => 136
	i32 3486566296, ; 583: System.Transactions => 0xcfd0c798 => 149
	i32 3493954962, ; 584: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 238
	i32 3509114376, ; 585: System.Xml.Linq => 0xd128d608 => 154
	i32 3515174580, ; 586: System.Security.dll => 0xd1854eb4 => 129
	i32 3530912306, ; 587: System.Configuration => 0xd2757232 => 19
	i32 3539954161, ; 588: System.Net.HttpListener => 0xd2ff69f1 => 64
	i32 3545306353, ; 589: Microsoft.Data.SqlClient => 0xd35114f1 => 180
	i32 3555084973, ; 590: de\Microsoft.Data.SqlClient.resources => 0xd3e64aad => 304
	i32 3558648585, ; 591: System.ClientModel => 0xd41cab09 => 213
	i32 3560100363, ; 592: System.Threading.Timer => 0xd432d20b => 146
	i32 3561949811, ; 593: Azure.Core.dll => 0xd44f0a73 => 172
	i32 3564312303, ; 594: Dapper.dll => 0xd47316ef => 177
	i32 3570554715, ; 595: System.IO.FileSystem.AccessControl => 0xd4d2575b => 46
	i32 3570608287, ; 596: System.Runtime.Caching.dll => 0xd4d3289f => 219
	i32 3580758918, ; 597: zh-HK\Microsoft.Maui.Controls.resources => 0xd56e0b86 => 345
	i32 3597029428, ; 598: Xamarin.Android.Glide.GifDecoder.dll => 0xd6665034 => 224
	i32 3598340787, ; 599: System.Net.WebSockets.Client => 0xd67a52b3 => 78
	i32 3608519521, ; 600: System.Linq.dll => 0xd715a361 => 60
	i32 3624195450, ; 601: System.Runtime.InteropServices.RuntimeInformation => 0xd804d57a => 105
	i32 3627220390, ; 602: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 273
	i32 3633644679, ; 603: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 228
	i32 3638274909, ; 604: System.IO.FileSystem.Primitives.dll => 0xd8dbab5d => 48
	i32 3641597786, ; 605: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 259
	i32 3643446276, ; 606: tr\Microsoft.Maui.Controls.resources => 0xd92a9404 => 342
	i32 3643854240, ; 607: Xamarin.AndroidX.Navigation.Fragment => 0xd930cda0 => 270
	i32 3645089577, ; 608: System.ComponentModel.DataAnnotations => 0xd943a729 => 14
	i32 3657292374, ; 609: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd9fdda56 => 182
	i32 3660523487, ; 610: System.Net.NetworkInformation => 0xda2f27df => 67
	i32 3663323240, ; 611: SkiaSharp.Skottie => 0xda59e068 => 209
	i32 3672681054, ; 612: Mono.Android.dll => 0xdae8aa5e => 170
	i32 3682565725, ; 613: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 234
	i32 3684561358, ; 614: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 238
	i32 3697841164, ; 615: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xdc68940c => 347
	i32 3700591436, ; 616: Microsoft.IdentityModel.Abstractions.dll => 0xdc928b4c => 192
	i32 3700866549, ; 617: System.Net.WebProxy.dll => 0xdc96bdf5 => 77
	i32 3706696989, ; 618: Xamarin.AndroidX.Core.Core.Ktx.dll => 0xdcefb51d => 243
	i32 3716563718, ; 619: System.Runtime.Intrinsics => 0xdd864306 => 107
	i32 3718780102, ; 620: Xamarin.AndroidX.Annotation => 0xdda814c6 => 227
	i32 3724971120, ; 621: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 269
	i32 3732100267, ; 622: System.Net.NameResolution => 0xde7354ab => 66
	i32 3737834244, ; 623: System.Net.Http.Json.dll => 0xdecad304 => 62
	i32 3748608112, ; 624: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 215
	i32 3751444290, ; 625: System.Xml.XPath => 0xdf9a7f42 => 159
	i32 3786282454, ; 626: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 236
	i32 3792276235, ; 627: System.Collections.NonGeneric => 0xe2098b0b => 10
	i32 3800979733, ; 628: Microsoft.Maui.Controls.Compatibility => 0xe28e5915 => 198
	i32 3802395368, ; 629: System.Collections.Specialized.dll => 0xe2a3f2e8 => 11
	i32 3803019198, ; 630: zh-Hans/Microsoft.Data.SqlClient.resources.dll => 0xe2ad77be => 312
	i32 3817368567, ; 631: CommunityToolkit.Maui.dll => 0xe3886bf7 => 174
	i32 3819260425, ; 632: System.Net.WebProxy => 0xe3a54a09 => 77
	i32 3823053215, ; 633: AppTransporte => 0xe3df299f => 0
	i32 3823082795, ; 634: System.Security.Cryptography.dll => 0xe3df9d2b => 125
	i32 3829621856, ; 635: System.Numerics.dll => 0xe4436460 => 82
	i32 3841636137, ; 636: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xe4fab729 => 184
	i32 3844307129, ; 637: System.Net.Mail.dll => 0xe52378b9 => 65
	i32 3848348906, ; 638: es\Microsoft.Data.SqlClient.resources => 0xe56124ea => 305
	i32 3849253459, ; 639: System.Runtime.InteropServices.dll => 0xe56ef253 => 106
	i32 3870376305, ; 640: System.Net.HttpListener.dll => 0xe6b14171 => 64
	i32 3873536506, ; 641: System.Security.Principal => 0xe6e179fa => 127
	i32 3875112723, ; 642: System.Security.Cryptography.Encoding.dll => 0xe6f98713 => 121
	i32 3885497537, ; 643: System.Net.WebHeaderCollection.dll => 0xe797fcc1 => 76
	i32 3885922214, ; 644: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 284
	i32 3888767677, ; 645: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0xe7c9e2bd => 274
	i32 3889960447, ; 646: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xe7dc15ff => 346
	i32 3896106733, ; 647: System.Collections.Concurrent.dll => 0xe839deed => 8
	i32 3896760992, ; 648: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 242
	i32 3901907137, ; 649: Microsoft.VisualBasic.Core.dll => 0xe89260c1 => 2
	i32 3920810846, ; 650: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 43
	i32 3921031405, ; 651: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 287
	i32 3928044579, ; 652: System.Xml.ReaderWriter => 0xea213423 => 155
	i32 3930554604, ; 653: System.Security.Principal.dll => 0xea4780ec => 127
	i32 3931092270, ; 654: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 272
	i32 3945713374, ; 655: System.Data.DataSetExtensions.dll => 0xeb2ecede => 23
	i32 3953953790, ; 656: System.Text.Encoding.CodePages => 0xebac8bfe => 132
	i32 3955647286, ; 657: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 230
	i32 3959773229, ; 658: Xamarin.AndroidX.Lifecycle.Process => 0xec05582d => 261
	i32 3980434154, ; 659: th/Microsoft.Maui.Controls.resources.dll => 0xed409aea => 341
	i32 3987592930, ; 660: he/Microsoft.Maui.Controls.resources.dll => 0xedadd6e2 => 323
	i32 4003436829, ; 661: System.Diagnostics.Process.dll => 0xee9f991d => 28
	i32 4015948917, ; 662: Xamarin.AndroidX.Annotation.Jvm.dll => 0xef5e8475 => 229
	i32 4025784931, ; 663: System.Memory => 0xeff49a63 => 61
	i32 4046471985, ; 664: Microsoft.Maui.Controls.Xaml.dll => 0xf1304331 => 200
	i32 4054681211, ; 665: System.Reflection.Emit.ILGeneration => 0xf1ad867b => 89
	i32 4068434129, ; 666: System.Private.Xml.Linq.dll => 0xf27f60d1 => 86
	i32 4073602200, ; 667: System.Threading.dll => 0xf2ce3c98 => 147
	i32 4094352644, ; 668: Microsoft.Maui.Essentials.dll => 0xf40add04 => 202
	i32 4099507663, ; 669: System.Drawing.dll => 0xf45985cf => 35
	i32 4100113165, ; 670: System.Private.Uri => 0xf462c30d => 85
	i32 4101593132, ; 671: Xamarin.AndroidX.Emoji2 => 0xf479582c => 250
	i32 4102112229, ; 672: pt/Microsoft.Maui.Controls.resources.dll => 0xf48143e5 => 336
	i32 4125707920, ; 673: ms/Microsoft.Maui.Controls.resources.dll => 0xf5e94e90 => 331
	i32 4126470640, ; 674: Microsoft.Extensions.DependencyInjection => 0xf5f4f1f0 => 183
	i32 4127667938, ; 675: System.IO.FileSystem.Watcher => 0xf60736e2 => 49
	i32 4130442656, ; 676: System.AppContext => 0xf6318da0 => 6
	i32 4147896353, ; 677: System.Reflection.Emit.ILGeneration.dll => 0xf73be021 => 89
	i32 4150914736, ; 678: uk\Microsoft.Maui.Controls.resources => 0xf769eeb0 => 343
	i32 4151237749, ; 679: System.Core => 0xf76edc75 => 21
	i32 4159265925, ; 680: System.Xml.XmlSerializer => 0xf7e95c85 => 161
	i32 4161255271, ; 681: System.Reflection.TypeExtensions => 0xf807b767 => 95
	i32 4164802419, ; 682: System.IO.FileSystem.Watcher.dll => 0xf83dd773 => 49
	i32 4181436372, ; 683: System.Runtime.Serialization.Primitives => 0xf93ba7d4 => 112
	i32 4182413190, ; 684: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 266
	i32 4185676441, ; 685: System.Security => 0xf97c5a99 => 129
	i32 4196529839, ; 686: System.Net.WebClient.dll => 0xfa21f6af => 75
	i32 4213026141, ; 687: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 215
	i32 4256097574, ; 688: Xamarin.AndroidX.Core.Core.Ktx => 0xfdaee526 => 243
	i32 4257443520, ; 689: ko/Microsoft.Data.SqlClient.resources.dll => 0xfdc36ec0 => 309
	i32 4258378803, ; 690: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 0xfdd1b433 => 265
	i32 4260525087, ; 691: System.Buffers => 0xfdf2741f => 7
	i32 4263231520, ; 692: System.IdentityModel.Tokens.Jwt.dll => 0xfe1bc020 => 217
	i32 4271975918, ; 693: Microsoft.Maui.Controls.dll => 0xfea12dee => 199
	i32 4274623895, ; 694: CommunityToolkit.Mvvm.dll => 0xfec99597 => 176
	i32 4274976490, ; 695: System.Runtime.Numerics => 0xfecef6ea => 109
	i32 4292120959, ; 696: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 266
	i32 4294763496 ; 697: Xamarin.AndroidX.ExifInterface.dll => 0xfffce3e8 => 252
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [698 x i32] [
	i32 67, ; 0
	i32 66, ; 1
	i32 107, ; 2
	i32 262, ; 3
	i32 296, ; 4
	i32 47, ; 5
	i32 79, ; 6
	i32 144, ; 7
	i32 307, ; 8
	i32 308, ; 9
	i32 29, ; 10
	i32 347, ; 11
	i32 123, ; 12
	i32 203, ; 13
	i32 101, ; 14
	i32 280, ; 15
	i32 106, ; 16
	i32 280, ; 17
	i32 138, ; 18
	i32 300, ; 19
	i32 308, ; 20
	i32 76, ; 21
	i32 123, ; 22
	i32 13, ; 23
	i32 236, ; 24
	i32 311, ; 25
	i32 131, ; 26
	i32 282, ; 27
	i32 150, ; 28
	i32 344, ; 29
	i32 345, ; 30
	i32 18, ; 31
	i32 234, ; 32
	i32 26, ; 33
	i32 256, ; 34
	i32 1, ; 35
	i32 58, ; 36
	i32 41, ; 37
	i32 90, ; 38
	i32 239, ; 39
	i32 312, ; 40
	i32 146, ; 41
	i32 258, ; 42
	i32 255, ; 43
	i32 316, ; 44
	i32 53, ; 45
	i32 68, ; 46
	i32 344, ; 47
	i32 225, ; 48
	i32 82, ; 49
	i32 204, ; 50
	i32 329, ; 51
	i32 257, ; 52
	i32 328, ; 53
	i32 130, ; 54
	i32 207, ; 55
	i32 54, ; 56
	i32 148, ; 57
	i32 73, ; 58
	i32 144, ; 59
	i32 206, ; 60
	i32 61, ; 61
	i32 145, ; 62
	i32 348, ; 63
	i32 164, ; 64
	i32 340, ; 65
	i32 240, ; 66
	i32 12, ; 67
	i32 253, ; 68
	i32 124, ; 69
	i32 151, ; 70
	i32 112, ; 71
	i32 165, ; 72
	i32 163, ; 73
	i32 255, ; 74
	i32 192, ; 75
	i32 268, ; 76
	i32 83, ; 77
	i32 327, ; 78
	i32 321, ; 79
	i32 208, ; 80
	i32 189, ; 81
	i32 205, ; 82
	i32 149, ; 83
	i32 300, ; 84
	i32 59, ; 85
	i32 185, ; 86
	i32 50, ; 87
	i32 102, ; 88
	i32 113, ; 89
	i32 179, ; 90
	i32 39, ; 91
	i32 293, ; 92
	i32 291, ; 93
	i32 119, ; 94
	i32 335, ; 95
	i32 174, ; 96
	i32 51, ; 97
	i32 43, ; 98
	i32 118, ; 99
	i32 245, ; 100
	i32 333, ; 101
	i32 251, ; 102
	i32 80, ; 103
	i32 135, ; 104
	i32 287, ; 105
	i32 232, ; 106
	i32 8, ; 107
	i32 72, ; 108
	i32 315, ; 109
	i32 154, ; 110
	i32 209, ; 111
	i32 302, ; 112
	i32 153, ; 113
	i32 91, ; 114
	i32 297, ; 115
	i32 44, ; 116
	i32 330, ; 117
	i32 318, ; 118
	i32 301, ; 119
	i32 108, ; 120
	i32 213, ; 121
	i32 128, ; 122
	i32 208, ; 123
	i32 25, ; 124
	i32 222, ; 125
	i32 71, ; 126
	i32 54, ; 127
	i32 45, ; 128
	i32 339, ; 129
	i32 207, ; 130
	i32 188, ; 131
	i32 246, ; 132
	i32 0, ; 133
	i32 22, ; 134
	i32 260, ; 135
	i32 85, ; 136
	i32 42, ; 137
	i32 159, ; 138
	i32 70, ; 139
	i32 273, ; 140
	i32 3, ; 141
	i32 41, ; 142
	i32 62, ; 143
	i32 16, ; 144
	i32 52, ; 145
	i32 342, ; 146
	i32 296, ; 147
	i32 104, ; 148
	i32 301, ; 149
	i32 294, ; 150
	i32 257, ; 151
	i32 33, ; 152
	i32 157, ; 153
	i32 84, ; 154
	i32 31, ; 155
	i32 12, ; 156
	i32 50, ; 157
	i32 55, ; 158
	i32 277, ; 159
	i32 35, ; 160
	i32 184, ; 161
	i32 317, ; 162
	i32 295, ; 163
	i32 230, ; 164
	i32 34, ; 165
	i32 57, ; 166
	i32 304, ; 167
	i32 264, ; 168
	i32 191, ; 169
	i32 178, ; 170
	i32 17, ; 171
	i32 298, ; 172
	i32 216, ; 173
	i32 163, ; 174
	i32 330, ; 175
	i32 263, ; 176
	i32 187, ; 177
	i32 180, ; 178
	i32 290, ; 179
	i32 336, ; 180
	i32 152, ; 181
	i32 286, ; 182
	i32 271, ; 183
	i32 334, ; 184
	i32 232, ; 185
	i32 28, ; 186
	i32 176, ; 187
	i32 51, ; 188
	i32 332, ; 189
	i32 291, ; 190
	i32 5, ; 191
	i32 316, ; 192
	i32 281, ; 193
	i32 285, ; 194
	i32 237, ; 195
	i32 302, ; 196
	i32 229, ; 197
	i32 248, ; 198
	i32 84, ; 199
	i32 290, ; 200
	i32 60, ; 201
	i32 111, ; 202
	i32 56, ; 203
	i32 346, ; 204
	i32 277, ; 205
	i32 98, ; 206
	i32 19, ; 207
	i32 241, ; 208
	i32 110, ; 209
	i32 100, ; 210
	i32 101, ; 211
	i32 314, ; 212
	i32 103, ; 213
	i32 294, ; 214
	i32 70, ; 215
	i32 37, ; 216
	i32 31, ; 217
	i32 102, ; 218
	i32 72, ; 219
	i32 217, ; 220
	i32 320, ; 221
	i32 9, ; 222
	i32 122, ; 223
	i32 45, ; 224
	i32 231, ; 225
	i32 189, ; 226
	i32 9, ; 227
	i32 42, ; 228
	i32 4, ; 229
	i32 278, ; 230
	i32 324, ; 231
	i32 193, ; 232
	i32 319, ; 233
	i32 30, ; 234
	i32 137, ; 235
	i32 91, ; 236
	i32 92, ; 237
	i32 339, ; 238
	i32 219, ; 239
	i32 48, ; 240
	i32 140, ; 241
	i32 111, ; 242
	i32 139, ; 243
	i32 173, ; 244
	i32 247, ; 245
	i32 114, ; 246
	i32 313, ; 247
	i32 295, ; 248
	i32 156, ; 249
	i32 75, ; 250
	i32 78, ; 251
	i32 267, ; 252
	i32 36, ; 253
	i32 211, ; 254
	i32 289, ; 255
	i32 196, ; 256
	i32 175, ; 257
	i32 251, ; 258
	i32 244, ; 259
	i32 63, ; 260
	i32 137, ; 261
	i32 15, ; 262
	i32 115, ; 263
	i32 283, ; 264
	i32 292, ; 265
	i32 239, ; 266
	i32 47, ; 267
	i32 69, ; 268
	i32 79, ; 269
	i32 125, ; 270
	i32 93, ; 271
	i32 120, ; 272
	i32 299, ; 273
	i32 26, ; 274
	i32 260, ; 275
	i32 206, ; 276
	i32 96, ; 277
	i32 27, ; 278
	i32 235, ; 279
	i32 337, ; 280
	i32 315, ; 281
	i32 148, ; 282
	i32 168, ; 283
	i32 4, ; 284
	i32 97, ; 285
	i32 32, ; 286
	i32 92, ; 287
	i32 282, ; 288
	i32 185, ; 289
	i32 21, ; 290
	i32 40, ; 291
	i32 169, ; 292
	i32 331, ; 293
	i32 253, ; 294
	i32 323, ; 295
	i32 190, ; 296
	i32 179, ; 297
	i32 267, ; 298
	i32 298, ; 299
	i32 292, ; 300
	i32 272, ; 301
	i32 2, ; 302
	i32 133, ; 303
	i32 110, ; 304
	i32 186, ; 305
	i32 343, ; 306
	i32 222, ; 307
	i32 340, ; 308
	i32 57, ; 309
	i32 94, ; 310
	i32 196, ; 311
	i32 322, ; 312
	i32 38, ; 313
	i32 233, ; 314
	i32 25, ; 315
	i32 93, ; 316
	i32 88, ; 317
	i32 98, ; 318
	i32 10, ; 319
	i32 86, ; 320
	i32 99, ; 321
	i32 279, ; 322
	i32 181, ; 323
	i32 299, ; 324
	i32 224, ; 325
	i32 197, ; 326
	i32 319, ; 327
	i32 7, ; 328
	i32 264, ; 329
	i32 314, ; 330
	i32 221, ; 331
	i32 191, ; 332
	i32 87, ; 333
	i32 259, ; 334
	i32 153, ; 335
	i32 318, ; 336
	i32 32, ; 337
	i32 115, ; 338
	i32 81, ; 339
	i32 20, ; 340
	i32 11, ; 341
	i32 161, ; 342
	i32 3, ; 343
	i32 201, ; 344
	i32 326, ; 345
	i32 188, ; 346
	i32 186, ; 347
	i32 83, ; 348
	i32 303, ; 349
	i32 63, ; 350
	i32 328, ; 351
	i32 286, ; 352
	i32 142, ; 353
	i32 310, ; 354
	i32 268, ; 355
	i32 156, ; 356
	i32 195, ; 357
	i32 40, ; 358
	i32 116, ; 359
	i32 182, ; 360
	i32 223, ; 361
	i32 322, ; 362
	i32 275, ; 363
	i32 130, ; 364
	i32 74, ; 365
	i32 65, ; 366
	i32 332, ; 367
	i32 171, ; 368
	i32 309, ; 369
	i32 227, ; 370
	i32 142, ; 371
	i32 105, ; 372
	i32 150, ; 373
	i32 69, ; 374
	i32 212, ; 375
	i32 155, ; 376
	i32 194, ; 377
	i32 181, ; 378
	i32 120, ; 379
	i32 126, ; 380
	i32 327, ; 381
	i32 151, ; 382
	i32 250, ; 383
	i32 140, ; 384
	i32 237, ; 385
	i32 324, ; 386
	i32 20, ; 387
	i32 14, ; 388
	i32 134, ; 389
	i32 74, ; 390
	i32 58, ; 391
	i32 240, ; 392
	i32 166, ; 393
	i32 167, ; 394
	i32 199, ; 395
	i32 15, ; 396
	i32 73, ; 397
	i32 6, ; 398
	i32 23, ; 399
	i32 262, ; 400
	i32 216, ; 401
	i32 221, ; 402
	i32 90, ; 403
	i32 325, ; 404
	i32 1, ; 405
	i32 135, ; 406
	i32 263, ; 407
	i32 285, ; 408
	i32 133, ; 409
	i32 68, ; 410
	i32 145, ; 411
	i32 334, ; 412
	i32 303, ; 413
	i32 254, ; 414
	i32 187, ; 415
	i32 87, ; 416
	i32 95, ; 417
	i32 244, ; 418
	i32 249, ; 419
	i32 212, ; 420
	i32 329, ; 421
	i32 30, ; 422
	i32 218, ; 423
	i32 44, ; 424
	i32 258, ; 425
	i32 194, ; 426
	i32 172, ; 427
	i32 220, ; 428
	i32 223, ; 429
	i32 108, ; 430
	i32 157, ; 431
	i32 34, ; 432
	i32 22, ; 433
	i32 173, ; 434
	i32 113, ; 435
	i32 56, ; 436
	i32 283, ; 437
	i32 143, ; 438
	i32 117, ; 439
	i32 119, ; 440
	i32 109, ; 441
	i32 225, ; 442
	i32 138, ; 443
	i32 231, ; 444
	i32 190, ; 445
	i32 53, ; 446
	i32 104, ; 447
	i32 335, ; 448
	i32 204, ; 449
	i32 200, ; 450
	i32 201, ; 451
	i32 132, ; 452
	i32 297, ; 453
	i32 288, ; 454
	i32 276, ; 455
	i32 341, ; 456
	i32 254, ; 457
	i32 210, ; 458
	i32 203, ; 459
	i32 158, ; 460
	i32 305, ; 461
	i32 320, ; 462
	i32 241, ; 463
	i32 162, ; 464
	i32 131, ; 465
	i32 276, ; 466
	i32 160, ; 467
	i32 333, ; 468
	i32 265, ; 469
	i32 307, ; 470
	i32 139, ; 471
	i32 288, ; 472
	i32 284, ; 473
	i32 168, ; 474
	i32 202, ; 475
	i32 220, ; 476
	i32 175, ; 477
	i32 226, ; 478
	i32 293, ; 479
	i32 39, ; 480
	i32 252, ; 481
	i32 80, ; 482
	i32 55, ; 483
	i32 36, ; 484
	i32 96, ; 485
	i32 165, ; 486
	i32 171, ; 487
	i32 210, ; 488
	i32 289, ; 489
	i32 81, ; 490
	i32 228, ; 491
	i32 97, ; 492
	i32 29, ; 493
	i32 158, ; 494
	i32 214, ; 495
	i32 177, ; 496
	i32 18, ; 497
	i32 126, ; 498
	i32 118, ; 499
	i32 248, ; 500
	i32 279, ; 501
	i32 261, ; 502
	i32 214, ; 503
	i32 281, ; 504
	i32 164, ; 505
	i32 311, ; 506
	i32 256, ; 507
	i32 218, ; 508
	i32 348, ; 509
	i32 278, ; 510
	i32 269, ; 511
	i32 169, ; 512
	i32 16, ; 513
	i32 143, ; 514
	i32 326, ; 515
	i32 197, ; 516
	i32 124, ; 517
	i32 117, ; 518
	i32 37, ; 519
	i32 114, ; 520
	i32 46, ; 521
	i32 141, ; 522
	i32 116, ; 523
	i32 33, ; 524
	i32 178, ; 525
	i32 313, ; 526
	i32 94, ; 527
	i32 52, ; 528
	i32 270, ; 529
	i32 128, ; 530
	i32 152, ; 531
	i32 24, ; 532
	i32 160, ; 533
	i32 247, ; 534
	i32 147, ; 535
	i32 103, ; 536
	i32 88, ; 537
	i32 235, ; 538
	i32 59, ; 539
	i32 141, ; 540
	i32 306, ; 541
	i32 310, ; 542
	i32 99, ; 543
	i32 5, ; 544
	i32 13, ; 545
	i32 121, ; 546
	i32 134, ; 547
	i32 27, ; 548
	i32 321, ; 549
	i32 193, ; 550
	i32 71, ; 551
	i32 245, ; 552
	i32 24, ; 553
	i32 205, ; 554
	i32 233, ; 555
	i32 306, ; 556
	i32 274, ; 557
	i32 271, ; 558
	i32 338, ; 559
	i32 136, ; 560
	i32 226, ; 561
	i32 242, ; 562
	i32 167, ; 563
	i32 195, ; 564
	i32 275, ; 565
	i32 317, ; 566
	i32 100, ; 567
	i32 122, ; 568
	i32 246, ; 569
	i32 183, ; 570
	i32 162, ; 571
	i32 166, ; 572
	i32 249, ; 573
	i32 38, ; 574
	i32 198, ; 575
	i32 325, ; 576
	i32 17, ; 577
	i32 211, ; 578
	i32 170, ; 579
	i32 338, ; 580
	i32 337, ; 581
	i32 136, ; 582
	i32 149, ; 583
	i32 238, ; 584
	i32 154, ; 585
	i32 129, ; 586
	i32 19, ; 587
	i32 64, ; 588
	i32 180, ; 589
	i32 304, ; 590
	i32 213, ; 591
	i32 146, ; 592
	i32 172, ; 593
	i32 177, ; 594
	i32 46, ; 595
	i32 219, ; 596
	i32 345, ; 597
	i32 224, ; 598
	i32 78, ; 599
	i32 60, ; 600
	i32 105, ; 601
	i32 273, ; 602
	i32 228, ; 603
	i32 48, ; 604
	i32 259, ; 605
	i32 342, ; 606
	i32 270, ; 607
	i32 14, ; 608
	i32 182, ; 609
	i32 67, ; 610
	i32 209, ; 611
	i32 170, ; 612
	i32 234, ; 613
	i32 238, ; 614
	i32 347, ; 615
	i32 192, ; 616
	i32 77, ; 617
	i32 243, ; 618
	i32 107, ; 619
	i32 227, ; 620
	i32 269, ; 621
	i32 66, ; 622
	i32 62, ; 623
	i32 215, ; 624
	i32 159, ; 625
	i32 236, ; 626
	i32 10, ; 627
	i32 198, ; 628
	i32 11, ; 629
	i32 312, ; 630
	i32 174, ; 631
	i32 77, ; 632
	i32 0, ; 633
	i32 125, ; 634
	i32 82, ; 635
	i32 184, ; 636
	i32 65, ; 637
	i32 305, ; 638
	i32 106, ; 639
	i32 64, ; 640
	i32 127, ; 641
	i32 121, ; 642
	i32 76, ; 643
	i32 284, ; 644
	i32 274, ; 645
	i32 346, ; 646
	i32 8, ; 647
	i32 242, ; 648
	i32 2, ; 649
	i32 43, ; 650
	i32 287, ; 651
	i32 155, ; 652
	i32 127, ; 653
	i32 272, ; 654
	i32 23, ; 655
	i32 132, ; 656
	i32 230, ; 657
	i32 261, ; 658
	i32 341, ; 659
	i32 323, ; 660
	i32 28, ; 661
	i32 229, ; 662
	i32 61, ; 663
	i32 200, ; 664
	i32 89, ; 665
	i32 86, ; 666
	i32 147, ; 667
	i32 202, ; 668
	i32 35, ; 669
	i32 85, ; 670
	i32 250, ; 671
	i32 336, ; 672
	i32 331, ; 673
	i32 183, ; 674
	i32 49, ; 675
	i32 6, ; 676
	i32 89, ; 677
	i32 343, ; 678
	i32 21, ; 679
	i32 161, ; 680
	i32 95, ; 681
	i32 49, ; 682
	i32 112, ; 683
	i32 266, ; 684
	i32 129, ; 685
	i32 75, ; 686
	i32 215, ; 687
	i32 243, ; 688
	i32 309, ; 689
	i32 265, ; 690
	i32 7, ; 691
	i32 217, ; 692
	i32 199, ; 693
	i32 176, ; 694
	i32 109, ; 695
	i32 266, ; 696
	i32 252 ; 697
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.2xx @ 0d97e20b84d8e87c3502469ee395805907905fe3"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"NumRegisterParameters", i32 0}
