using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"CommandLine.dll",
		"Newtonsoft.Json.dll",
		"Sirenix.Utilities.dll",
		"System.Core.dll",
		"UnityEngine.AssetBundleModule.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// <>f__AnonymousType16<int,object>
	// <>f__AnonymousType17<int,object>
	// <>f__AnonymousType1<object,object>
	// CSharpx.Just<object>
	// CSharpx.Maybe<object>
	// CSharpx.Nothing<object>
	// CommandLine.Core.InstanceBuilder.<>c__0<object>
	// CommandLine.Core.InstanceBuilder.<>c__DisplayClass0_0<object>
	// CommandLine.Core.InstanceBuilder.<>c__DisplayClass0_1<object>
	// CommandLine.Core.ReflectionExtensions.<>c__0<object>
	// CommandLine.Core.ReflectionExtensions.<>c__DisplayClass0_0<object>
	// CommandLine.NotParsed<object>
	// CommandLine.Parsed<object>
	// CommandLine.Parser.<>c__DisplayClass17_0<object>
	// CommandLine.ParserResult<object>
	// System.Action<System.Guid>
	// System.Action<UnityEngine.Vector2>
	// System.Action<int>
	// System.Action<long,long>
	// System.Action<object,object>
	// System.Action<object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<System.Guid>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<System.Guid>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<System.Guid,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<System.Guid,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<System.Guid,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<System.Guid,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<System.Guid,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary<System.Guid,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.EqualityComparer<System.Guid>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.Guid,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.Guid>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<System.Guid>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Guid,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.Guid>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.Guid,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.Guid>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<System.Guid>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IList<System.Guid>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<System.Guid,object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.List.Enumerator<System.Guid>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<System.Guid>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<System.Guid>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<System.Guid>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Guid>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<System.Guid>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<int>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,byte,object>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<CastIterator>d__99<object>
	// System.Linq.Enumerable.<OfTypeIterator>d__97<object>
	// System.Linq.Enumerable.<SelectManyIterator>d__17<object,object>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.GroupedEnumerable<object,object,object>
	// System.Linq.Lookup.<GetEnumerator>d__12<object,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<object,object>
	// System.Linq.Lookup.Grouping<object,object>
	// System.Linq.Lookup<object,object>
	// System.Nullable<long>
	// System.Predicate<System.Guid>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<int>
	// System.Threading.Tasks.TaskFactory<object>
	// System.Tuple<object,object,object>
	// System.Tuple<object,object>
	// UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene,int>
	// }}

	public void RefMethods()
	{
		// System.Collections.Generic.IEnumerable<object> CSharpx.EnumerableExtensions.Memoize<object>(System.Collections.Generic.IEnumerable<object>)
		// CSharpx.Just<object> CSharpx.Maybe.Just<object>(object)
		// CSharpx.Maybe<object> CSharpx.Maybe.Nothing<object>()
		// object CSharpx.MaybeExtensions.MapValueOrDefault<object,object>(CSharpx.Maybe<object>,System.Func<object,object>,object)
		// CommandLine.ParserResult<object> CommandLine.Core.InstanceBuilder.Build<object>(CSharpx.Maybe<System.Func<object>>,System.Func<System.Collections.Generic.IEnumerable<string>,System.Collections.Generic.IEnumerable<CommandLine.Core.OptionSpecification>,RailwaySharp.ErrorHandling.Result<System.Collections.Generic.IEnumerable<CommandLine.Core.Token>,CommandLine.Error>>,System.Collections.Generic.IEnumerable<string>,System.StringComparer,bool,System.Globalization.CultureInfo,bool,bool,System.Collections.Generic.IEnumerable<CommandLine.ErrorType>)
		// System.Collections.Generic.IEnumerable<object> CommandLine.Core.ReflectionExtensions.GetSpecifications<object>(System.Type,System.Func<System.Reflection.PropertyInfo,object>)
		// RailwaySharp.ErrorHandling.Result<System.Collections.Generic.IEnumerable<CommandLine.Core.Token>,CommandLine.Error> CommandLine.Parser.<ParseArguments>b__11_0<object>(System.Collections.Generic.IEnumerable<string>,System.Collections.Generic.IEnumerable<CommandLine.Core.OptionSpecification>)
		// CommandLine.ParserResult<object> CommandLine.Parser.DisplayHelp<object>(CommandLine.ParserResult<object>,System.IO.TextWriter,int)
		// CommandLine.ParserResult<object> CommandLine.Parser.MakeParserResult<object>(CommandLine.ParserResult<object>,CommandLine.ParserSettings)
		// CommandLine.ParserResult<object> CommandLine.Parser.ParseArguments<object>(System.Collections.Generic.IEnumerable<string>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithNotParsed<object>(CommandLine.ParserResult<object>,System.Action<System.Collections.Generic.IEnumerable<CommandLine.Error>>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithParsed<object>(CommandLine.ParserResult<object>,System.Action<object>)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// bool Sirenix.Utilities.LinqExtensions.IsNullOrEmpty<object>(System.Collections.Generic.IList<object>)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Empty<object>()
		// object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<object,object>> System.Linq.Enumerable.GroupBy<object,object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>,System.Func<object,object>)
		// object System.Linq.Enumerable.Last<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfType<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfTypeIterator<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectMany<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectManyIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__12>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter<int>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__12>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter<int>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.Net.DownloaderV2.<DownloadAsync>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d>(Sunflower.Net.DownloaderV2.<>c__DisplayClass12_0.<<DownloadAsync>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d>(Sunflower.Net.DownloaderV2.<>c__DisplayClass13_2.<<DownloadAsync>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.Net.DownloaderV2.<DownloadAsync>d__11>(Sunflower.Net.DownloaderV2.<DownloadAsync>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.Net.DownloaderV2.<DownloadAsync>d__12>(Sunflower.Net.DownloaderV2.<DownloadAsync>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.Net.DownloaderV2.<DownloadAsync>d__13>(Sunflower.Net.DownloaderV2.<DownloadAsync>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14>(Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadAssetBundleAsync>d__14&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13>(Sunflower.PatchMgr.Runtime.PatchMgrV2.<DownloadCompareFile>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<Awake>d__5>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<Awake>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Sunflower.PatchMgr.Runtime.PatchMgrV2.<OnEventPatchEventCompareFileDownloadSuccessAsync>d__9>(System.Runtime.CompilerServices.TaskAwaiter&,Sunflower.PatchMgr.Runtime.PatchMgrV2.<OnEventPatchEventCompareFileDownloadSuccessAsync>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7>(Sunflower.PatchManager.Runtime.PatchMgr.<CompareAB>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Sunflower.PatchMgr.Runtime.PatchMgrV2.<Awake>d__5>(Sunflower.PatchMgr.Runtime.PatchMgrV2.<Awake>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<Sunflower.PatchMgr.Runtime.PatchMgrV2.<OnEventPatchEventCompareFileDownloadSuccessAsync>d__9>(Sunflower.PatchMgr.Runtime.PatchMgrV2.<OnEventPatchEventCompareFileDownloadSuccessAsync>d__9&)
		// object[] UnityEngine.AssetBundle.ConvertObjects<object>(UnityEngine.Object[])
		// object[] UnityEngine.AssetBundle.LoadAllAssets<object>()
		// object UnityEngine.AssetBundle.LoadAsset<object>(string)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Resources.Load<object>(string)
	}
}