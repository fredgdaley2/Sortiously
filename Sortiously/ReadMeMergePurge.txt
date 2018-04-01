

Merge Purge

MergePurge takes two files. First one is the master. Second one is the source, aka detail.
There are four file scenarios that can be handled.

1) Master Delimited and a Delimited Source
2) Master Delimited and a Fixed Width Source
3) Master Fixed Width and a Fixed Width Source
4) Master Fixed Width and a Delimited Source


There are two modes of data processing.

1) Active: Creates a sorted file from the master by a defined key and sort direction. There is the
		   ability to Add, Update, Delete and Ignore data from the detail source. The created master's
		   data will reflect the changes.

2) Passive: Creates a sorted file from the master by a defined key and sort direction. Unlike the Active
			mode the Add, Update, Delete and Ignore does not affect the newly created master file instead
			it create a file associated with each action and the data from the detail source in the detail's file
			format. So for example any adds will go into an adds file which will be in the details file format.


FYI: The original master and detail files are never altered.
	 Any files being created will be auto cleaned up if there is an exception.


Each method returns an MergePurgeResults object.

public class MergePurgeResults
{
	//The number of added records to the master in either Active or Passive mode.
	public int AddCount { get; set; }

	//The number of deleted records to the master in either Active or Passive mode.
	public int DeleteCount { get; set; }

	//The number of updated records to the master in either Active or Passive mode.
	public int UpdateCount { get; set; }

	//The number of ignored records from the details file that did not affect the master in either Active or Passive mode.
	public int IgnoreCount { get; set; }

	//The number of key matches between the master and detail in either Active or Passive mode.
	public int MatchesCount { get; set; }

	//The file path to the Adds file only populated in Passive mode
	//The name is the detail file name suffixed with "_adds" and the detail's extension
	public string AddsFilePath { get; set; }

	//The file path to the Deletes file only populated in Passive mode
	//The name is the detail file name suffixed with "_deletes" and the detail's extension
	public string DeletesFilePath { get; set; }

	//The file path to the Updates file only populated in Passive mode
	//The name is the detail file name suffixed with "_updates" and the detail's extension
	public string UpdatesFilePath { get; set; }

	//The file path to the Ignored file only populated in Passive mode
	//The name is the detail file name suffixed with "_ignored" and the detail's extension
	public string IgnoredFilePath { get; set; }

	//The file path to the newly created master file in either Active or Passive mode.
	//The name is the master file name suffixed with "_master" and the masters's extension
	public string NewMasterFilePath { get; set; }
}


Master Delimited / Delimited Source
public static MergePurgeResults MergePurge<T>(MasterDelimitedFileSource<T> master,
											  DelimitedFileSource<T> detail,
											  Action<MergePurgeParam> processData,
											  string destinationFolder = null,
											  DataMode processMode = DataMode.Passive)

Master Delimited / Fixed Width Source
public static MergePurgeResults MergePurge<T>(MasterDelimitedFileSource<T> master,
											  FixedWidthFileSource<T> detail,
											  Action<MergePurgeParam> processData,
											  string destinationFolder = null,
											  DataMode processMode = DataMode.Passive)
Master Fixed Width / Fixed Width Source
public static MergePurgeResults MergePurge<T>(MasterFixedWidthFileSource<T> master,
											  FixedWidthFileSource<T> detail,
											  Action<MergePurgeParam> processData,
											  string destinationFolder = null,
											  DataMode processMode = DataMode.Passive)
Master Fixed Width / Delimited Source
public static MergePurgeResults MergePurge<T>(MasterFixedWidthFileSource<T> master,
											  DelimitedFileSource<T> detail,
											  Action<MergePurgeParam> processData,
											  string destinationFolder = null,
											  DataMode processMode = DataMode.Passive)

Each method will need a method that accepts a MergePurgeParm object for its processData action.

Here is an example of a processData action method.

public class MergePurgeParam
{
	//the fields from the master parsed using the textfield parser.
	public string[] MasterFields { get; set; }

	//the fields from the source detail parsed using the textfield parser.
	public string[] DetailFields { get; set; }

	//if the defined key from detail exists in the master, true or false
	public bool KeyFound { get; set; }

	//the action to take on the data determined by the logic in the processData action method.
	public MergePurgeAction DataAction { get; set; }
}


static void MockDataProcess(MergePurgeParam mpParam)
{

	//just an example, for every detail key found in master update fields 1 & 2
	if (mpParam.MasterFields != null)
	{
		mpParam.MasterFields[1] = "Hello";
		mpParam.MasterFields[2] = "World";
		mpParam.DataAction = MergePurgeAction.Update;
	}

	//just showing some logic to demonstrate delete action
	if (mpParam.KeyFound && mpParam.DetailFields[0] == "699")
	{
		mpParam.DataAction = MergePurgeAction.Delete;
	}

	//if not found in master add it from the detail data
	if (!mpParam.KeyFound)
	{
		mpParam.MasterFields = mpParam.DetailFields;
		mpParam.DataAction = MergePurgeAction.Add;
	}
}

Rules for the DataAction:
If a key is not found the only actions that can and will take place are Add and Ignore.
if a key is found the only actions that can and will take place are Update, Delete and Ignore.




Example 1) Master Delimited / Delimited Source

static void MockDataMergePurge()
{
	MergePurgeResults mpResults = MasterDetail.MergePurge<string>(
	new MasterDelimitedFileSource<string>
	{
		SourceFilePath = Path.Combine(masterSourceFolder, "MockData.csv"),
		Delimiter = Delimiters.Comma,
		HasHeader = true,
		GetStringKey = (fields, line) => fields[0].PadKey(10),
		SortDirection = SortDirection.Ascending,
		MaxBatchSize = 100000   //Defaulted to 250000 if not supplied
	},
	new DelimitedFileSource<string>
	{
		SourceFilePath = Path.Combine(masterSourceFolder, "MockDataDetail.csv"),
		Delimiter = Delimiters.Comma,
		HasHeader = true,
		GetStringKey = (fields, line) => fields[0].PadKey(10)
	},
	MockDataProcess,
	masterDestFolder,
	DataMode.Active);
}

//The processData action method
static void MockDataProcess(MergePurgeParam mpParam)
{

	//just an example for every detail key found in master update fields 1 & 2
	if (mpParam.MasterFields != null)
	{
		mpParam.MasterFields[1] = "Hello";
		mpParam.MasterFields[2] = "World";
		mpParam.DataAction = MergePurgeAction.Update;
	}

	//just showing some logic to demonstrate delete action
	if (mpParam.KeyFound && mpParam.DetailFields[0] == "699")
	{
		mpParam.DataAction = MergePurgeAction.Delete;
	}

	//if not found in master add it from the detail data
	if (!mpParam.KeyFound)
	{
		mpParam.MasterFields = mpParam.DetailFields;
		mpParam.DataAction = MergePurgeAction.Add;
	}
}


Example 2) Master Delimited / FixedWidth Source

static void MockDataMergePurge_FWDetail()
{
	MergePurgeResults mpResults = MasterDetail.MergePurge<string>(new MasterDelimitedFileSource<string>
	{
		SourceFilePath = Path.Combine(masterSourceFolder, "MockData.csv"),
		Delimiter = Delimiters.Comma,
		HasHeader = true,
		GetStringKey = (fields, line) => fields[0].PadKey(5, '0'),
		SortDirection = SortDirection.Ascending
		MaxBatchSize = 100000   //Defaulted to 250000 if not supplied
	},
		new FixedWidthFileSource<string>
		{
			SourceFilePath = Path.Combine(masterSourceFolder, "FWMockDataDetail.txt"),
			FixedWidths = new int[] { 5, 9, 10, 32, 7, 15, 20, 10 },
			HasHeader = false,
			GetStringKey = (line) => line.Substring(0, 5).PadKey(5, '0')
		},
		MockDataFwDetailProcess,
		masterDestFolder,
		DataMode.Passive);
}


Example 3) Master Fixed Width / Delimited Source

static void MockDataMergePurge_FWMaster()
{
	MergePurgeResults mpResults = MasterDetail.MergePurge<long>(new MasterFixedWidthFileSource<long>
	{
		SourceFilePath = Path.Combine(masterSourceFolder, "FWMockDataDetail.txt"),
		FixedWidths = new int[] { 5, 9, 10, 32, 7, 15, 20, 10 },
		HasHeader = false,
		GetNumKey = (line) => long.Parse(line.Substring(0, 5).Trim()),
		SortDirection = SortDirection.Ascending
		MaxBatchSize = 100000   //Defaulted to 250000 if not supplied
	},
		new DelimitedFileSource<long>
		{
			SourceFilePath = Path.Combine(masterSourceFolder, "MockData.csv"),
			Delimiter = Delimiters.Comma,
			HasHeader = true,
			GetNumKey = (fields, line) => long.Parse(fields[0].Trim())
		},
		MockDataFwDetailProcess,
		masterDestFolder,
		DataMode.Active);
}


//The processData action method for examples 2 and 3.
static void MockDataFwDetailProcess(MergePurgeParam mpParam)
{
	//just adding any new data
	if (!mpParam.KeyFound)
	{
		mpParam.MasterFields = mpParam.DetailFields;
		mpParam.DataAction = MergePurgeAction.Add;
	}
}


Hint: For fixed width master files when added or updated the
	  master fields will be left aligned when the record is
	  constructed according to the fixed widths. if right
	  alignment or specialized alignment is desired then
	  do the alignment to the master field(s) in the processData
	  action method.
