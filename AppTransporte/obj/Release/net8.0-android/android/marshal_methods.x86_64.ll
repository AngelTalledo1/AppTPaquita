; ModuleID = 'marshal_methods.x86_64.ll'
source_filename = "marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [123 x ptr] zeroinitializer, align 16

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [246 x i64] [
	i64 98382396393917666, ; 0: Microsoft.Extensions.Primitives.dll => 0x15d8644ad360ce2 => 42
	i64 120698629574877762, ; 1: Mono.Android => 0x1accec39cafe242 => 122
	i64 131669012237370309, ; 2: Microsoft.Maui.Essentials.dll => 0x1d3c844de55c3c5 => 46
	i64 196720943101637631, ; 3: System.Linq.Expressions.dll => 0x2bae4a7cd73f3ff => 99
	i64 210515253464952879, ; 4: Xamarin.AndroidX.Collection.dll => 0x2ebe681f694702f => 60
	i64 232391251801502327, ; 5: Xamarin.AndroidX.SavedState.dll => 0x3399e9cbc897277 => 77
	i64 308826992458506653, ; 6: SkiaSharp.Extended.dll => 0x4492c836e8aa19d => 49
	i64 545109961164950392, ; 7: fi/Microsoft.Maui.Controls.resources.dll => 0x7909e9f1ec38b78 => 7
	i64 750875890346172408, ; 8: System.Threading.Thread => 0xa6ba5a4da7d1ff8 => 114
	i64 799765834175365804, ; 9: System.ComponentModel.dll => 0xb1956c9f18442ac => 92
	i64 849051935479314978, ; 10: hi/Microsoft.Maui.Controls.resources.dll => 0xbc8703ca21a3a22 => 10
	i64 872800313462103108, ; 11: Xamarin.AndroidX.DrawerLayout => 0xc1ccf42c3c21c44 => 65
	i64 1120440138749646132, ; 12: Xamarin.Google.Android.Material.dll => 0xf8c9a5eae431534 => 81
	i64 1121665720830085036, ; 13: nb/Microsoft.Maui.Controls.resources.dll => 0xf90f507becf47ac => 18
	i64 1268860745194512059, ; 14: System.Drawing.dll => 0x119be62002c19ebb => 96
	i64 1369545283391376210, ; 15: Xamarin.AndroidX.Navigation.Fragment.dll => 0x13019a2dd85acb52 => 73
	i64 1433520707554318520, ; 16: SkiaSharp.Extended.UI.dll => 0x13e4e37d07f118b8 => 50
	i64 1476839205573959279, ; 17: System.Net.Primitives.dll => 0x147ec96ece9b1e6f => 103
	i64 1486715745332614827, ; 18: Microsoft.Maui.Controls.dll => 0x14a1e017ea87d6ab => 43
	i64 1513467482682125403, ; 19: Mono.Android.Runtime => 0x1500eaa8245f6c5b => 121
	i64 1537168428375924959, ; 20: System.Threading.Thread.dll => 0x15551e8a954ae0df => 114
	i64 1556147632182429976, ; 21: ko/Microsoft.Maui.Controls.resources.dll => 0x15988c06d24c8918 => 16
	i64 1624659445732251991, ; 22: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0x168bf32877da9957 => 58
	i64 1628611045998245443, ; 23: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0x1699fd1e1a00b643 => 70
	i64 1743969030606105336, ; 24: System.Memory.dll => 0x1833d297e88f2af8 => 101
	i64 1767386781656293639, ; 25: System.Private.Uri.dll => 0x188704e9f5582107 => 107
	i64 1795316252682057001, ; 26: Xamarin.AndroidX.AppCompat.dll => 0x18ea3e9eac997529 => 57
	i64 1835311033149317475, ; 27: es\Microsoft.Maui.Controls.resources => 0x197855a927386163 => 6
	i64 1836611346387731153, ; 28: Xamarin.AndroidX.SavedState => 0x197cf449ebe482d1 => 77
	i64 1875417405349196092, ; 29: System.Drawing.Primitives => 0x1a06d2319b6c713c => 95
	i64 1881198190668717030, ; 30: tr\Microsoft.Maui.Controls.resources => 0x1a1b5bc992ea9be6 => 28
	i64 1920760634179481754, ; 31: Microsoft.Maui.Controls.Xaml => 0x1aa7e99ec2d2709a => 44
	i64 1959996714666907089, ; 32: tr/Microsoft.Maui.Controls.resources.dll => 0x1b334ea0a2a755d1 => 28
	i64 1981742497975770890, ; 33: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x1b80904d5c241f0a => 69
	i64 1983698669889758782, ; 34: cs/Microsoft.Maui.Controls.resources.dll => 0x1b87836e2031a63e => 2
	i64 2019660174692588140, ; 35: pl/Microsoft.Maui.Controls.resources.dll => 0x1c07463a6f8e1a6c => 20
	i64 2067084732092737905, ; 36: AppTransporte => 0x1cafc29ca9056971 => 84
	i64 2102659300918482391, ; 37: System.Drawing.Primitives.dll => 0x1d2e257e6aead5d7 => 95
	i64 2262844636196693701, ; 38: Xamarin.AndroidX.DrawerLayout.dll => 0x1f673d352266e6c5 => 65
	i64 2287834202362508563, ; 39: System.Collections.Concurrent => 0x1fc00515e8ce7513 => 86
	i64 2302323944321350744, ; 40: ru/Microsoft.Maui.Controls.resources.dll => 0x1ff37f6ddb267c58 => 24
	i64 2329709569556905518, ; 41: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x2054ca829b447e2e => 68
	i64 2470498323731680442, ; 42: Xamarin.AndroidX.CoordinatorLayout => 0x2248f922dc398cba => 61
	i64 2497223385847772520, ; 43: System.Runtime => 0x22a7eb7046413568 => 111
	i64 2547086958574651984, ; 44: Xamarin.AndroidX.Activity.dll => 0x2359121801df4a50 => 56
	i64 2602673633151553063, ; 45: th\Microsoft.Maui.Controls.resources => 0x241e8de13a460e27 => 27
	i64 2656907746661064104, ; 46: Microsoft.Extensions.DependencyInjection => 0x24df3b84c8b75da8 => 37
	i64 2662981627730767622, ; 47: cs\Microsoft.Maui.Controls.resources => 0x24f4cfae6c48af06 => 2
	i64 2895129759130297543, ; 48: fi\Microsoft.Maui.Controls.resources => 0x282d912d479fa4c7 => 7
	i64 3017704767998173186, ; 49: Xamarin.Google.Android.Material => 0x29e10a7f7d88a002 => 81
	i64 3289520064315143713, ; 50: Xamarin.AndroidX.Lifecycle.Common => 0x2da6b911e3063621 => 67
	i64 3311221304742556517, ; 51: System.Numerics.Vectors.dll => 0x2df3d23ba9e2b365 => 105
	i64 3344514922410554693, ; 52: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x2e6a1a9a18463545 => 83
	i64 3414639567687375782, ; 53: SkiaSharp.Views.Maui.Controls => 0x2f633c9863ffdba6 => 54
	i64 3429672777697402584, ; 54: Microsoft.Maui.Essentials => 0x2f98a5385a7b1ed8 => 46
	i64 3494946837667399002, ; 55: Microsoft.Extensions.Configuration => 0x30808ba1c00a455a => 35
	i64 3522470458906976663, ; 56: Xamarin.AndroidX.SwipeRefreshLayout => 0x30e2543832f52197 => 78
	i64 3551103847008531295, ; 57: System.Private.CoreLib.dll => 0x31480e226177735f => 119
	i64 3567343442040498961, ; 58: pt\Microsoft.Maui.Controls.resources => 0x3181bff5bea4ab11 => 22
	i64 3571415421602489686, ; 59: System.Runtime.dll => 0x319037675df7e556 => 111
	i64 3638003163729360188, ; 60: Microsoft.Extensions.Configuration.Abstractions => 0x327cc89a39d5f53c => 36
	i64 3647754201059316852, ; 61: System.Xml.ReaderWriter => 0x329f6d1e86145474 => 116
	i64 3655542548057982301, ; 62: Microsoft.Extensions.Configuration.dll => 0x32bb18945e52855d => 35
	i64 3716579019761409177, ; 63: netstandard.dll => 0x3393f0ed5c8c5c99 => 118
	i64 3727469159507183293, ; 64: Xamarin.AndroidX.RecyclerView => 0x33baa1739ba646bd => 76
	i64 3869221888984012293, ; 65: Microsoft.Extensions.Logging.dll => 0x35b23cceda0ed605 => 39
	i64 3890352374528606784, ; 66: Microsoft.Maui.Controls.Xaml.dll => 0x35fd4edf66e00240 => 44
	i64 3933965368022646939, ; 67: System.Net.Requests => 0x369840a8bfadc09b => 104
	i64 3966267475168208030, ; 68: System.Memory => 0x370b03412596249e => 101
	i64 4073500526318903918, ; 69: System.Private.Xml.dll => 0x3887fb25779ae26e => 108
	i64 4120493066591692148, ; 70: zh-Hant\Microsoft.Maui.Controls.resources => 0x392eee9cdda86574 => 33
	i64 4154383907710350974, ; 71: System.ComponentModel => 0x39a7562737acb67e => 92
	i64 4187479170553454871, ; 72: System.Linq.Expressions => 0x3a1cea1e912fa117 => 99
	i64 4205801962323029395, ; 73: System.ComponentModel.TypeConverter => 0x3a5e0299f7e7ad93 => 91
	i64 4306612231831054753, ; 74: SkiaSharp.SceneGraph.dll => 0x3bc42901e7a469a1 => 51
	i64 4356591372459378815, ; 75: vi/Microsoft.Maui.Controls.resources.dll => 0x3c75b8c562f9087f => 30
	i64 4679594760078841447, ; 76: ar/Microsoft.Maui.Controls.resources.dll => 0x40f142a407475667 => 0
	i64 4794310189461587505, ; 77: Xamarin.AndroidX.Activity => 0x4288cfb749e4c631 => 56
	i64 4795410492532947900, ; 78: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0x428cb86f8f9b7bbc => 78
	i64 4809057822547766521, ; 79: System.Drawing => 0x42bd349c3145ecf9 => 96
	i64 4853321196694829351, ; 80: System.Runtime.Loader.dll => 0x435a75ea15de7927 => 110
	i64 5103417709280584325, ; 81: System.Collections.Specialized => 0x46d2fb5e161b6285 => 88
	i64 5182934613077526976, ; 82: System.Collections.Specialized.dll => 0x47ed7b91fa9009c0 => 88
	i64 5290786973231294105, ; 83: System.Runtime.Loader => 0x496ca6b869b72699 => 110
	i64 5471532531798518949, ; 84: sv\Microsoft.Maui.Controls.resources => 0x4beec9d926d82ca5 => 26
	i64 5522859530602327440, ; 85: uk\Microsoft.Maui.Controls.resources => 0x4ca5237b51eead90 => 29
	i64 5570799893513421663, ; 86: System.IO.Compression.Brotli => 0x4d4f74fcdfa6c35f => 97
	i64 5573260873512690141, ; 87: System.Security.Cryptography.dll => 0x4d58333c6e4ea1dd => 112
	i64 5692067934154308417, ; 88: Xamarin.AndroidX.ViewPager2.dll => 0x4efe49a0d4a8bb41 => 80
	i64 6068057819846744445, ; 89: ro/Microsoft.Maui.Controls.resources.dll => 0x5436126fec7f197d => 23
	i64 6200764641006662125, ; 90: ro\Microsoft.Maui.Controls.resources => 0x560d8a96830131ed => 23
	i64 6268464631992009879, ; 91: SkiaSharp.Skottie => 0x56fe0f5efcfbc497 => 52
	i64 6357457916754632952, ; 92: _Microsoft.Android.Resource.Designer => 0x583a3a4ac2a7a0f8 => 34
	i64 6401687960814735282, ; 93: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0x58d75d486341cfb2 => 68
	i64 6478287442656530074, ; 94: hr\Microsoft.Maui.Controls.resources => 0x59e7801b0c6a8e9a => 11
	i64 6548213210057960872, ; 95: Xamarin.AndroidX.CustomView.dll => 0x5adfed387b066da8 => 64
	i64 6560151584539558821, ; 96: Microsoft.Extensions.Options => 0x5b0a571be53243a5 => 41
	i64 6671798237668743565, ; 97: SkiaSharp => 0x5c96fd260152998d => 48
	i64 6743165466166707109, ; 98: nl\Microsoft.Maui.Controls.resources => 0x5d948943c08c43a5 => 19
	i64 6777482997383978746, ; 99: pt/Microsoft.Maui.Controls.resources.dll => 0x5e0e74e0a2525efa => 22
	i64 6894844156784520562, ; 100: System.Numerics.Vectors => 0x5faf683aead1ad72 => 105
	i64 7220009545223068405, ; 101: sv/Microsoft.Maui.Controls.resources.dll => 0x6432a06d99f35af5 => 26
	i64 7270811800166795866, ; 102: System.Linq => 0x64e71ccf51a90a5a => 100
	i64 7314237870106916923, ; 103: SkiaSharp.Views.Maui.Core.dll => 0x65816497226eb83b => 55
	i64 7377312882064240630, ; 104: System.ComponentModel.TypeConverter.dll => 0x66617afac45a2ff6 => 91
	i64 7489048572193775167, ; 105: System.ObjectModel => 0x67ee71ff6b419e3f => 106
	i64 7654504624184590948, ; 106: System.Net.Http => 0x6a3a4366801b8264 => 102
	i64 7708790323521193081, ; 107: ms/Microsoft.Maui.Controls.resources.dll => 0x6afb1ff4d1730479 => 17
	i64 7714652370974252055, ; 108: System.Private.CoreLib => 0x6b0ff375198b9c17 => 119
	i64 7723873813026311384, ; 109: SkiaSharp.Views.Maui.Controls.dll => 0x6b30b64f63600cd8 => 54
	i64 7735352534559001595, ; 110: Xamarin.Kotlin.StdLib.dll => 0x6b597e2582ce8bfb => 82
	i64 7836164640616011524, ; 111: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x6cbfa6390d64d704 => 58
	i64 7924623109668182241, ; 112: AppTransporte.dll => 0x6df9eabb569f98e1 => 84
	i64 7927939710195668715, ; 113: SkiaSharp.Views.Android.dll => 0x6e05b32992ed16eb => 53
	i64 8064050204834738623, ; 114: System.Collections.dll => 0x6fe942efa61731bf => 89
	i64 8083354569033831015, ; 115: Xamarin.AndroidX.Lifecycle.Common.dll => 0x702dd82730cad267 => 67
	i64 8087206902342787202, ; 116: System.Diagnostics.DiagnosticSource => 0x703b87d46f3aa082 => 94
	i64 8167236081217502503, ; 117: Java.Interop.dll => 0x7157d9f1a9b8fd27 => 120
	i64 8185542183669246576, ; 118: System.Collections => 0x7198e33f4794aa70 => 89
	i64 8246048515196606205, ; 119: Microsoft.Maui.Graphics.dll => 0x726fd96f64ee56fd => 47
	i64 8368701292315763008, ; 120: System.Security.Cryptography => 0x7423997c6fd56140 => 112
	i64 8400357532724379117, ; 121: Xamarin.AndroidX.Navigation.UI.dll => 0x749410ab44503ded => 75
	i64 8563666267364444763, ; 122: System.Private.Uri => 0x76d841191140ca5b => 107
	i64 8614108721271900878, ; 123: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x778b763e14018ace => 21
	i64 8626175481042262068, ; 124: Java.Interop => 0x77b654e585b55834 => 120
	i64 8639588376636138208, ; 125: Xamarin.AndroidX.Navigation.Runtime => 0x77e5fbdaa2fda2e0 => 74
	i64 8677882282824630478, ; 126: pt-BR\Microsoft.Maui.Controls.resources => 0x786e07f5766b00ce => 21
	i64 8725526185868997716, ; 127: System.Diagnostics.DiagnosticSource.dll => 0x79174bd613173454 => 94
	i64 9045785047181495996, ; 128: zh-HK\Microsoft.Maui.Controls.resources => 0x7d891592e3cb0ebc => 31
	i64 9312692141327339315, ; 129: Xamarin.AndroidX.ViewPager2 => 0x813d54296a634f33 => 80
	i64 9324707631942237306, ; 130: Xamarin.AndroidX.AppCompat => 0x8168042fd44a7c7a => 57
	i64 9334813198578103615, ; 131: SkiaSharp.Extended.UI => 0x818beb2569e0353f => 50
	i64 9659729154652888475, ; 132: System.Text.RegularExpressions => 0x860e407c9991dd9b => 113
	i64 9678050649315576968, ; 133: Xamarin.AndroidX.CoordinatorLayout.dll => 0x864f57c9feb18c88 => 61
	i64 9702891218465930390, ; 134: System.Collections.NonGeneric.dll => 0x86a79827b2eb3c96 => 87
	i64 9808709177481450983, ; 135: Mono.Android.dll => 0x881f890734e555e7 => 122
	i64 9956195530459977388, ; 136: Microsoft.Maui => 0x8a2b8315b36616ac => 45
	i64 9991543690424095600, ; 137: es/Microsoft.Maui.Controls.resources.dll => 0x8aa9180c89861370 => 6
	i64 10038780035334861115, ; 138: System.Net.Http.dll => 0x8b50e941206af13b => 102
	i64 10051358222726253779, ; 139: System.Private.Xml => 0x8b7d990c97ccccd3 => 108
	i64 10092835686693276772, ; 140: Microsoft.Maui.Controls => 0x8c10f49539bd0c64 => 43
	i64 10143853363526200146, ; 141: da\Microsoft.Maui.Controls.resources => 0x8cc634e3c2a16b52 => 3
	i64 10229024438826829339, ; 142: Xamarin.AndroidX.CustomView => 0x8df4cb880b10061b => 64
	i64 10406448008575299332, ; 143: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x906b2153fcb3af04 => 83
	i64 10430153318873392755, ; 144: Xamarin.AndroidX.Core => 0x90bf592ea44f6673 => 62
	i64 10506226065143327199, ; 145: ca\Microsoft.Maui.Controls.resources => 0x91cd9cf11ed169df => 1
	i64 10785150219063592792, ; 146: System.Net.Primitives => 0x95ac8cfb68830758 => 103
	i64 11002576679268595294, ; 147: Microsoft.Extensions.Logging.Abstractions => 0x98b1013215cd365e => 40
	i64 11009005086950030778, ; 148: Microsoft.Maui.dll => 0x98c7d7cc621ffdba => 45
	i64 11103970607964515343, ; 149: hu\Microsoft.Maui.Controls.resources => 0x9a193a6fc41a6c0f => 12
	i64 11162124722117608902, ; 150: Xamarin.AndroidX.ViewPager => 0x9ae7d54b986d05c6 => 79
	i64 11220793807500858938, ; 151: ja\Microsoft.Maui.Controls.resources => 0x9bb8448481fdd63a => 15
	i64 11226290749488709958, ; 152: Microsoft.Extensions.Options.dll => 0x9bcbcbf50c874146 => 41
	i64 11340910727871153756, ; 153: Xamarin.AndroidX.CursorAdapter => 0x9d630238642d465c => 63
	i64 11485890710487134646, ; 154: System.Runtime.InteropServices => 0x9f6614bf0f8b71b6 => 109
	i64 11518296021396496455, ; 155: id\Microsoft.Maui.Controls.resources => 0x9fd9353475222047 => 13
	i64 11529969570048099689, ; 156: Xamarin.AndroidX.ViewPager.dll => 0xa002ae3c4dc7c569 => 79
	i64 11530571088791430846, ; 157: Microsoft.Extensions.Logging => 0xa004d1504ccd66be => 39
	i64 11597940890313164233, ; 158: netstandard => 0xa0f429ca8d1805c9 => 118
	i64 11705530742807338875, ; 159: he/Microsoft.Maui.Controls.resources.dll => 0xa272663128721f7b => 9
	i64 12102847907131387746, ; 160: System.Buffers => 0xa7f5f40c43256f62 => 85
	i64 12451044538927396471, ; 161: Xamarin.AndroidX.Fragment.dll => 0xaccaff0a2955b677 => 66
	i64 12466513435562512481, ; 162: Xamarin.AndroidX.Loader.dll => 0xad01f3eb52569061 => 71
	i64 12475113361194491050, ; 163: _Microsoft.Android.Resource.Designer.dll => 0xad2081818aba1caa => 34
	i64 12538491095302438457, ; 164: Xamarin.AndroidX.CardView.dll => 0xae01ab382ae67e39 => 59
	i64 12550732019250633519, ; 165: System.IO.Compression => 0xae2d28465e8e1b2f => 98
	i64 12681088699309157496, ; 166: it/Microsoft.Maui.Controls.resources.dll => 0xaffc46fc178aec78 => 14
	i64 12700543734426720211, ; 167: Xamarin.AndroidX.Collection => 0xb041653c70d157d3 => 60
	i64 12823819093633476069, ; 168: th/Microsoft.Maui.Controls.resources.dll => 0xb1f75b85abe525e5 => 27
	i64 12843321153144804894, ; 169: Microsoft.Extensions.Primitives => 0xb23ca48abd74d61e => 42
	i64 13221551921002590604, ; 170: ca/Microsoft.Maui.Controls.resources.dll => 0xb77c636bdebe318c => 1
	i64 13222659110913276082, ; 171: ja/Microsoft.Maui.Controls.resources.dll => 0xb78052679c1178b2 => 15
	i64 13343850469010654401, ; 172: Mono.Android.Runtime.dll => 0xb92ee14d854f44c1 => 121
	i64 13381594904270902445, ; 173: he\Microsoft.Maui.Controls.resources => 0xb9b4f9aaad3e94ad => 9
	i64 13465488254036897740, ; 174: Xamarin.Kotlin.StdLib => 0xbadf06394d106fcc => 82
	i64 13467053111158216594, ; 175: uk/Microsoft.Maui.Controls.resources.dll => 0xbae49573fde79792 => 29
	i64 13540124433173649601, ; 176: vi\Microsoft.Maui.Controls.resources => 0xbbe82f6eede718c1 => 30
	i64 13545416393490209236, ; 177: id/Microsoft.Maui.Controls.resources.dll => 0xbbfafc7174bc99d4 => 13
	i64 13572454107664307259, ; 178: Xamarin.AndroidX.RecyclerView.dll => 0xbc5b0b19d99f543b => 76
	i64 13717397318615465333, ; 179: System.ComponentModel.Primitives.dll => 0xbe5dfc2ef2f87d75 => 90
	i64 13755568601956062840, ; 180: fr/Microsoft.Maui.Controls.resources.dll => 0xbee598c36b1b9678 => 8
	i64 13814445057219246765, ; 181: hr/Microsoft.Maui.Controls.resources.dll => 0xbfb6c49664b43aad => 11
	i64 13881769479078963060, ; 182: System.Console.dll => 0xc0a5f3cade5c6774 => 93
	i64 13959074834287824816, ; 183: Xamarin.AndroidX.Fragment => 0xc1b8989a7ad20fb0 => 66
	i64 14100563506285742564, ; 184: da/Microsoft.Maui.Controls.resources.dll => 0xc3af43cd0cff89e4 => 3
	i64 14124974489674258913, ; 185: Xamarin.AndroidX.CardView => 0xc405fd76067d19e1 => 59
	i64 14125464355221830302, ; 186: System.Threading.dll => 0xc407bafdbc707a9e => 115
	i64 14461014870687870182, ; 187: System.Net.Requests.dll => 0xc8afd8683afdece6 => 104
	i64 14464374589798375073, ; 188: ru\Microsoft.Maui.Controls.resources => 0xc8bbc80dcb1e5ea1 => 24
	i64 14522721392235705434, ; 189: el/Microsoft.Maui.Controls.resources.dll => 0xc98b12295c2cf45a => 5
	i64 14552901170081803662, ; 190: SkiaSharp.Views.Maui.Core => 0xc9f64a827617ad8e => 55
	i64 14669215534098758659, ; 191: Microsoft.Extensions.DependencyInjection.dll => 0xcb9385ceb3993c03 => 37
	i64 14705122255218365489, ; 192: ko\Microsoft.Maui.Controls.resources => 0xcc1316c7b0fb5431 => 16
	i64 14744092281598614090, ; 193: zh-Hans\Microsoft.Maui.Controls.resources => 0xcc9d89d004439a4a => 32
	i64 14852515768018889994, ; 194: Xamarin.AndroidX.CursorAdapter.dll => 0xce1ebc6625a76d0a => 63
	i64 14892012299694389861, ; 195: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xceab0e490a083a65 => 33
	i64 14904040806490515477, ; 196: ar\Microsoft.Maui.Controls.resources => 0xced5ca2604cb2815 => 0
	i64 14954917835170835695, ; 197: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xcf8a8a895a82ecef => 38
	i64 14987728460634540364, ; 198: System.IO.Compression.dll => 0xcfff1ba06622494c => 98
	i64 15076659072870671916, ; 199: System.ObjectModel.dll => 0xd13b0d8c1620662c => 106
	i64 15101927338945785474, ; 200: SkiaSharp.SceneGraph => 0xd194d2e6bd9fae82 => 51
	i64 15111608613780139878, ; 201: ms\Microsoft.Maui.Controls.resources => 0xd1b737f831192f66 => 17
	i64 15115185479366240210, ; 202: System.IO.Compression.Brotli.dll => 0xd1c3ed1c1bc467d2 => 97
	i64 15133485256822086103, ; 203: System.Linq.dll => 0xd204f0a9127dd9d7 => 100
	i64 15227001540531775957, ; 204: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd3512d3999b8e9d5 => 36
	i64 15370334346939861994, ; 205: Xamarin.AndroidX.Core.dll => 0xd54e65a72c560bea => 62
	i64 15391712275433856905, ; 206: Microsoft.Extensions.DependencyInjection.Abstractions => 0xd59a58c406411f89 => 38
	i64 15527772828719725935, ; 207: System.Console => 0xd77dbb1e38cd3d6f => 93
	i64 15536481058354060254, ; 208: de\Microsoft.Maui.Controls.resources => 0xd79cab34eec75bde => 4
	i64 15582737692548360875, ; 209: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xd841015ed86f6aab => 70
	i64 15609085926864131306, ; 210: System.dll => 0xd89e9cf3334914ea => 117
	i64 15661133872274321916, ; 211: System.Xml.ReaderWriter.dll => 0xd9578647d4bfb1fc => 116
	i64 15664356999916475676, ; 212: de/Microsoft.Maui.Controls.resources.dll => 0xd962f9b2b6ecd51c => 4
	i64 15743187114543869802, ; 213: hu/Microsoft.Maui.Controls.resources.dll => 0xda7b09450ae4ef6a => 12
	i64 15783653065526199428, ; 214: el\Microsoft.Maui.Controls.resources => 0xdb0accd674b1c484 => 5
	i64 16154507427712707110, ; 215: System => 0xe03056ea4e39aa26 => 117
	i64 16288847719894691167, ; 216: nb\Microsoft.Maui.Controls.resources => 0xe20d9cb300c12d5f => 18
	i64 16321164108206115771, ; 217: Microsoft.Extensions.Logging.Abstractions.dll => 0xe2806c487e7b0bbb => 40
	i64 16324796876805858114, ; 218: SkiaSharp.dll => 0xe28d5444586b6342 => 48
	i64 16649148416072044166, ; 219: Microsoft.Maui.Graphics => 0xe70da84600bb4e86 => 47
	i64 16677317093839702854, ; 220: Xamarin.AndroidX.Navigation.UI => 0xe771bb8960dd8b46 => 75
	i64 16890310621557459193, ; 221: System.Text.RegularExpressions.dll => 0xea66700587f088f9 => 113
	i64 16942731696432749159, ; 222: sk\Microsoft.Maui.Controls.resources => 0xeb20acb622a01a67 => 25
	i64 16961387572093531548, ; 223: SkiaSharp.Extended => 0xeb62f421ac5c359c => 49
	i64 16998075588627545693, ; 224: Xamarin.AndroidX.Navigation.Fragment => 0xebe54bb02d623e5d => 73
	i64 17008137082415910100, ; 225: System.Collections.NonGeneric => 0xec090a90408c8cd4 => 87
	i64 17031351772568316411, ; 226: Xamarin.AndroidX.Navigation.Common.dll => 0xec5b843380a769fb => 72
	i64 17062143951396181894, ; 227: System.ComponentModel.Primitives => 0xecc8e986518c9786 => 90
	i64 17089008752050867324, ; 228: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xed285aeb25888c7c => 32
	i64 17342750010158924305, ; 229: hi\Microsoft.Maui.Controls.resources => 0xf0add33f97ecc211 => 10
	i64 17438153253682247751, ; 230: sk/Microsoft.Maui.Controls.resources.dll => 0xf200c3fe308d7847 => 25
	i64 17514990004910432069, ; 231: fr\Microsoft.Maui.Controls.resources => 0xf311be9c6f341f45 => 8
	i64 17623389608345532001, ; 232: pl\Microsoft.Maui.Controls.resources => 0xf492db79dfbef661 => 20
	i64 17671790519499593115, ; 233: SkiaSharp.Views.Android => 0xf53ecfd92be3959b => 53
	i64 17702523067201099846, ; 234: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xf5abfef008ae1846 => 31
	i64 17704177640604968747, ; 235: Xamarin.AndroidX.Loader => 0xf5b1dfc36cac272b => 71
	i64 17710060891934109755, ; 236: Xamarin.AndroidX.Lifecycle.ViewModel => 0xf5c6c68c9e45303b => 69
	i64 17712670374920797664, ; 237: System.Runtime.InteropServices.dll => 0xf5d00bdc38bd3de0 => 109
	i64 17808848867378959707, ; 238: SkiaSharp.Skottie.dll => 0xf725bdb086bd955b => 52
	i64 17838668724098252521, ; 239: System.Buffers.dll => 0xf78faeb0f5bf3ee9 => 85
	i64 18025913125965088385, ; 240: System.Threading => 0xfa28e87b91334681 => 115
	i64 18099568558057551825, ; 241: nl/Microsoft.Maui.Controls.resources.dll => 0xfb2e95b53ad977d1 => 19
	i64 18121036031235206392, ; 242: Xamarin.AndroidX.Navigation.Common => 0xfb7ada42d3d42cf8 => 72
	i64 18245806341561545090, ; 243: System.Collections.Concurrent.dll => 0xfd3620327d587182 => 86
	i64 18305135509493619199, ; 244: Xamarin.AndroidX.Navigation.Runtime.dll => 0xfe08e7c2d8c199ff => 74
	i64 18324163916253801303 ; 245: it\Microsoft.Maui.Controls.resources => 0xfe4c81ff0a56ab57 => 14
], align 16

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [246 x i32] [
	i32 42, ; 0
	i32 122, ; 1
	i32 46, ; 2
	i32 99, ; 3
	i32 60, ; 4
	i32 77, ; 5
	i32 49, ; 6
	i32 7, ; 7
	i32 114, ; 8
	i32 92, ; 9
	i32 10, ; 10
	i32 65, ; 11
	i32 81, ; 12
	i32 18, ; 13
	i32 96, ; 14
	i32 73, ; 15
	i32 50, ; 16
	i32 103, ; 17
	i32 43, ; 18
	i32 121, ; 19
	i32 114, ; 20
	i32 16, ; 21
	i32 58, ; 22
	i32 70, ; 23
	i32 101, ; 24
	i32 107, ; 25
	i32 57, ; 26
	i32 6, ; 27
	i32 77, ; 28
	i32 95, ; 29
	i32 28, ; 30
	i32 44, ; 31
	i32 28, ; 32
	i32 69, ; 33
	i32 2, ; 34
	i32 20, ; 35
	i32 84, ; 36
	i32 95, ; 37
	i32 65, ; 38
	i32 86, ; 39
	i32 24, ; 40
	i32 68, ; 41
	i32 61, ; 42
	i32 111, ; 43
	i32 56, ; 44
	i32 27, ; 45
	i32 37, ; 46
	i32 2, ; 47
	i32 7, ; 48
	i32 81, ; 49
	i32 67, ; 50
	i32 105, ; 51
	i32 83, ; 52
	i32 54, ; 53
	i32 46, ; 54
	i32 35, ; 55
	i32 78, ; 56
	i32 119, ; 57
	i32 22, ; 58
	i32 111, ; 59
	i32 36, ; 60
	i32 116, ; 61
	i32 35, ; 62
	i32 118, ; 63
	i32 76, ; 64
	i32 39, ; 65
	i32 44, ; 66
	i32 104, ; 67
	i32 101, ; 68
	i32 108, ; 69
	i32 33, ; 70
	i32 92, ; 71
	i32 99, ; 72
	i32 91, ; 73
	i32 51, ; 74
	i32 30, ; 75
	i32 0, ; 76
	i32 56, ; 77
	i32 78, ; 78
	i32 96, ; 79
	i32 110, ; 80
	i32 88, ; 81
	i32 88, ; 82
	i32 110, ; 83
	i32 26, ; 84
	i32 29, ; 85
	i32 97, ; 86
	i32 112, ; 87
	i32 80, ; 88
	i32 23, ; 89
	i32 23, ; 90
	i32 52, ; 91
	i32 34, ; 92
	i32 68, ; 93
	i32 11, ; 94
	i32 64, ; 95
	i32 41, ; 96
	i32 48, ; 97
	i32 19, ; 98
	i32 22, ; 99
	i32 105, ; 100
	i32 26, ; 101
	i32 100, ; 102
	i32 55, ; 103
	i32 91, ; 104
	i32 106, ; 105
	i32 102, ; 106
	i32 17, ; 107
	i32 119, ; 108
	i32 54, ; 109
	i32 82, ; 110
	i32 58, ; 111
	i32 84, ; 112
	i32 53, ; 113
	i32 89, ; 114
	i32 67, ; 115
	i32 94, ; 116
	i32 120, ; 117
	i32 89, ; 118
	i32 47, ; 119
	i32 112, ; 120
	i32 75, ; 121
	i32 107, ; 122
	i32 21, ; 123
	i32 120, ; 124
	i32 74, ; 125
	i32 21, ; 126
	i32 94, ; 127
	i32 31, ; 128
	i32 80, ; 129
	i32 57, ; 130
	i32 50, ; 131
	i32 113, ; 132
	i32 61, ; 133
	i32 87, ; 134
	i32 122, ; 135
	i32 45, ; 136
	i32 6, ; 137
	i32 102, ; 138
	i32 108, ; 139
	i32 43, ; 140
	i32 3, ; 141
	i32 64, ; 142
	i32 83, ; 143
	i32 62, ; 144
	i32 1, ; 145
	i32 103, ; 146
	i32 40, ; 147
	i32 45, ; 148
	i32 12, ; 149
	i32 79, ; 150
	i32 15, ; 151
	i32 41, ; 152
	i32 63, ; 153
	i32 109, ; 154
	i32 13, ; 155
	i32 79, ; 156
	i32 39, ; 157
	i32 118, ; 158
	i32 9, ; 159
	i32 85, ; 160
	i32 66, ; 161
	i32 71, ; 162
	i32 34, ; 163
	i32 59, ; 164
	i32 98, ; 165
	i32 14, ; 166
	i32 60, ; 167
	i32 27, ; 168
	i32 42, ; 169
	i32 1, ; 170
	i32 15, ; 171
	i32 121, ; 172
	i32 9, ; 173
	i32 82, ; 174
	i32 29, ; 175
	i32 30, ; 176
	i32 13, ; 177
	i32 76, ; 178
	i32 90, ; 179
	i32 8, ; 180
	i32 11, ; 181
	i32 93, ; 182
	i32 66, ; 183
	i32 3, ; 184
	i32 59, ; 185
	i32 115, ; 186
	i32 104, ; 187
	i32 24, ; 188
	i32 5, ; 189
	i32 55, ; 190
	i32 37, ; 191
	i32 16, ; 192
	i32 32, ; 193
	i32 63, ; 194
	i32 33, ; 195
	i32 0, ; 196
	i32 38, ; 197
	i32 98, ; 198
	i32 106, ; 199
	i32 51, ; 200
	i32 17, ; 201
	i32 97, ; 202
	i32 100, ; 203
	i32 36, ; 204
	i32 62, ; 205
	i32 38, ; 206
	i32 93, ; 207
	i32 4, ; 208
	i32 70, ; 209
	i32 117, ; 210
	i32 116, ; 211
	i32 4, ; 212
	i32 12, ; 213
	i32 5, ; 214
	i32 117, ; 215
	i32 18, ; 216
	i32 40, ; 217
	i32 48, ; 218
	i32 47, ; 219
	i32 75, ; 220
	i32 113, ; 221
	i32 25, ; 222
	i32 49, ; 223
	i32 73, ; 224
	i32 87, ; 225
	i32 72, ; 226
	i32 90, ; 227
	i32 32, ; 228
	i32 10, ; 229
	i32 25, ; 230
	i32 8, ; 231
	i32 20, ; 232
	i32 53, ; 233
	i32 31, ; 234
	i32 71, ; 235
	i32 69, ; 236
	i32 109, ; 237
	i32 52, ; 238
	i32 85, ; 239
	i32 115, ; 240
	i32 19, ; 241
	i32 72, ; 242
	i32 86, ; 243
	i32 74, ; 244
	i32 14 ; 245
], align 16

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

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
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 16

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.2xx @ 0d97e20b84d8e87c3502469ee395805907905fe3"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
