using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Configuration;
using Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig;


namespace Qoollo.PerformanceCounters.Configuration
{

    /* **** Sample section group *****
    
    public class SampleSectionGroup: ConfigurationSectionGroup
    {
    	public PerfCountersConfigurationSectionConfigClass PerfCountersConfigurationSectionSection
    	{
    		get
    		{
    			return this.Sections["PerfCountersConfigurationSectionSection"] as PerfCountersConfigurationSectionConfigClass;
    		}
    	}
    
    	public IPerfCountersConfigurationSection LoadPerfCountersConfigurationSectionSection()
    	{
    		return this.PerfCountersConfigurationSectionSection.ExtractConfigData();
    	}
    }
    
    */
    
    
    
    		
    /* **** Sample config loader ***** 
    
    public static class PerfCountersConfigurationSectionAppConfigLoader
    {
        public const string DefaultSectionName = "PerfCountersConfigurationSectionSection";
    
        public static IPerfCountersConfigurationSection Load(string sectionGroupName, string sectionName)
        {
            string fullSectionName = sectionName;
            if (sectionGroupName != null)
                fullSectionName = sectionGroupName + "/" + sectionName;
    
            var cfg = (PerfCountersConfigurationSectionConfigClass)ConfigurationManager.GetSection(fullSectionName);
            return cfg.ExtractConfigData();
        }
        public static IPerfCountersConfigurationSection Load(string sectionName)
        {
            return Load(null, sectionName);
        }
        public static IPerfCountersConfigurationSection Load()
        {
            return Load(DefaultSectionName);
        }
    }
    
    */
    
    
    
    		
    // ============================

    internal class PerfCountersConfigurationSectionImplement : IPerfCountersConfigurationSection
    {
        public PerfCountersConfigurationSectionImplement()
        {
            this._rootCounters = new PerfCountersConfigurationImplement();
        }

        private IPerfCountersConfiguration _rootCounters;
        public IPerfCountersConfiguration GetRootCountersVal()
        {
            return _rootCounters;
        }
        public void SetRootCountersVal(IPerfCountersConfiguration value)
        {
            _rootCounters = value;
        }
        IPerfCountersConfiguration IPerfCountersConfigurationSection.RootCounters
        {
            get { return _rootCounters; }
        }


        public PerfCountersConfigurationSectionImplement Copy()
        {
        	var res = new PerfCountersConfigurationSectionImplement();
        
            res._rootCounters = PerfCountersConfigurationImplement.CopyInh(this._rootCounters);

            return res;
        }

        public static IPerfCountersConfigurationSection CopyInh(IPerfCountersConfigurationSection src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(PerfCountersConfigurationSectionImplement))
        		return ((PerfCountersConfigurationSectionImplement)src).Copy();
        	if (src.GetType() == typeof(PerfCountersConfigurationSectionImplement))
        		return ((PerfCountersConfigurationSectionImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class PerfCountersConfigurationImplement : IPerfCountersConfiguration
    {
        public PerfCountersConfigurationImplement()
        {
        }


        public PerfCountersConfigurationImplement Copy()
        {
        	var res = new PerfCountersConfigurationImplement();
        

            return res;
        }

        public static IPerfCountersConfiguration CopyInh(IPerfCountersConfiguration src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(PerfCountersConfigurationImplement))
        		return ((PerfCountersConfigurationImplement)src).Copy();
        	if (src.GetType() == typeof(CompositeCountersImplement))
        		return ((CompositeCountersImplement)src).Copy();
        	if (src.GetType() == typeof(NetCountersImplement))
        		return ((NetCountersImplement)src).Copy();
        	if (src.GetType() == typeof(GraphiteCountersImplement))
        		return ((GraphiteCountersImplement)src).Copy();
        	if (src.GetType() == typeof(InternalCountersImplement))
        		return ((InternalCountersImplement)src).Copy();
        	if (src.GetType() == typeof(NullCountersImplement))
        		return ((NullCountersImplement)src).Copy();
        	if (src.GetType() == typeof(WinCountersImplement))
        		return ((WinCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class CompositeCountersImplement : ICompositeCounters
    {
        public CompositeCountersImplement()
        {
            this._wrappedCounters = new List<IPerfCountersConfiguration>();
        }

        private List<IPerfCountersConfiguration> _wrappedCounters;
        public List<IPerfCountersConfiguration> GetWrappedCountersVal()
        {
            return _wrappedCounters;
        }
        public void SetWrappedCountersVal(List<IPerfCountersConfiguration> value)
        {
            _wrappedCounters = value;
        }
        List<IPerfCountersConfiguration> ICompositeCounters.WrappedCounters
        {
            get { return _wrappedCounters; }
        }


        public CompositeCountersImplement Copy()
        {
        	var res = new CompositeCountersImplement();
        
            if (this._wrappedCounters == null)
                res._wrappedCounters = null;
            else
                res._wrappedCounters = _wrappedCounters.Select(o => PerfCountersConfigurationImplement.CopyInh(o)).ToList();


            return res;
        }

        public static ICompositeCounters CopyInh(ICompositeCounters src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(CompositeCountersImplement))
        		return ((CompositeCountersImplement)src).Copy();
        	if (src.GetType() == typeof(CompositeCountersImplement))
        		return ((CompositeCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class NetCountersImplement : INetCounters
    {
        public NetCountersImplement()
        {
            this._distributionPeriodMs = 1000;
            this._serverPort = 26115;
        }

        private Int32 _distributionPeriodMs;
        public Int32 GetDistributionPeriodMsVal()
        {
            return _distributionPeriodMs;
        }
        public void SetDistributionPeriodMsVal(Int32 value)
        {
            _distributionPeriodMs = value;
        }
        Int32 INetCounters.DistributionPeriodMs
        {
            get { return _distributionPeriodMs; }
        }

        private String _serverAddress;
        public String GetServerAddressVal()
        {
            return _serverAddress;
        }
        public void SetServerAddressVal(String value)
        {
            _serverAddress = value;
        }
        String INetCounters.ServerAddress
        {
            get { return _serverAddress; }
        }

        private Int32 _serverPort;
        public Int32 GetServerPortVal()
        {
            return _serverPort;
        }
        public void SetServerPortVal(Int32 value)
        {
            _serverPort = value;
        }
        Int32 INetCounters.ServerPort
        {
            get { return _serverPort; }
        }


        public NetCountersImplement Copy()
        {
        	var res = new NetCountersImplement();
        
            res._distributionPeriodMs = this._distributionPeriodMs;
            res._serverAddress = this._serverAddress;
            res._serverPort = this._serverPort;

            return res;
        }

        public static INetCounters CopyInh(INetCounters src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(NetCountersImplement))
        		return ((NetCountersImplement)src).Copy();
        	if (src.GetType() == typeof(NetCountersImplement))
        		return ((NetCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class GraphiteCountersImplement : IGraphiteCounters
    {
        public GraphiteCountersImplement()
        {
            this._distributionPeriodMs = 1000;
            this._namePrefixFormatString = "{MachineName}.{ProcessName}";
            this._serverPort = 2003;
        }

        private Int32 _distributionPeriodMs;
        public Int32 GetDistributionPeriodMsVal()
        {
            return _distributionPeriodMs;
        }
        public void SetDistributionPeriodMsVal(Int32 value)
        {
            _distributionPeriodMs = value;
        }
        Int32 IGraphiteCounters.DistributionPeriodMs
        {
            get { return _distributionPeriodMs; }
        }

        private String _namePrefixFormatString;
        public String GetNamePrefixFormatStringVal()
        {
            return _namePrefixFormatString;
        }
        public void SetNamePrefixFormatStringVal(String value)
        {
            _namePrefixFormatString = value;
        }
        String IGraphiteCounters.NamePrefixFormatString
        {
            get { return _namePrefixFormatString; }
        }

        private String _serverAddress;
        public String GetServerAddressVal()
        {
            return _serverAddress;
        }
        public void SetServerAddressVal(String value)
        {
            _serverAddress = value;
        }
        String IGraphiteCounters.ServerAddress
        {
            get { return _serverAddress; }
        }

        private Int32 _serverPort;
        public Int32 GetServerPortVal()
        {
            return _serverPort;
        }
        public void SetServerPortVal(Int32 value)
        {
            _serverPort = value;
        }
        Int32 IGraphiteCounters.ServerPort
        {
            get { return _serverPort; }
        }


        public GraphiteCountersImplement Copy()
        {
        	var res = new GraphiteCountersImplement();
        
            res._distributionPeriodMs = this._distributionPeriodMs;
            res._namePrefixFormatString = this._namePrefixFormatString;
            res._serverAddress = this._serverAddress;
            res._serverPort = this._serverPort;

            return res;
        }

        public static IGraphiteCounters CopyInh(IGraphiteCounters src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(GraphiteCountersImplement))
        		return ((GraphiteCountersImplement)src).Copy();
        	if (src.GetType() == typeof(GraphiteCountersImplement))
        		return ((GraphiteCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class InternalCountersImplement : IInternalCounters
    {
        public InternalCountersImplement()
        {
        }


        public InternalCountersImplement Copy()
        {
        	var res = new InternalCountersImplement();
        

            return res;
        }

        public static IInternalCounters CopyInh(IInternalCounters src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(InternalCountersImplement))
        		return ((InternalCountersImplement)src).Copy();
        	if (src.GetType() == typeof(InternalCountersImplement))
        		return ((InternalCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class NullCountersImplement : INullCounters
    {
        public NullCountersImplement()
        {
        }


        public NullCountersImplement Copy()
        {
        	var res = new NullCountersImplement();
        

            return res;
        }

        public static INullCounters CopyInh(INullCounters src)
        {
        	if (src == null)
        		return null;
        
        	if (src.GetType() == typeof(NullCountersImplement))
        		return ((NullCountersImplement)src).Copy();
        	if (src.GetType() == typeof(NullCountersImplement))
        		return ((NullCountersImplement)src).Copy();
        
        	throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }

    internal class WinCountersImplement : IWinCounters
    {
        public WinCountersImplement()
        {
            this._instantiationMode = WinCountersInstantiationModeCfg.UseExistedIfPossible;
            this._categoryNamePrefix = "";
            this._machineName = ".";
            this._isReadOnlyCounters = false;
            this._preferedBitness = WinCountersPreferedBitnessCfg.SameAsOperatingSystemBitness;
            this._existedInstancesTreatment = WinCountersExistedInstancesTreatmentCfg.LoadExisted;
        }

        private WinCountersInstantiationModeCfg _instantiationMode;
        public WinCountersInstantiationModeCfg GetInstantiationModeVal()
        {
            return _instantiationMode;
        }
        public void SetInstantiationModeVal(WinCountersInstantiationModeCfg value)
        {
            _instantiationMode = value;
        }
        WinCountersInstantiationModeCfg IWinCounters.InstantiationMode
        {
            get { return _instantiationMode; }
        }

        private String _categoryNamePrefix;
        public String GetCategoryNamePrefixVal()
        {
            return _categoryNamePrefix;
        }
        public void SetCategoryNamePrefixVal(String value)
        {
            _categoryNamePrefix = value;
        }
        String IWinCounters.CategoryNamePrefix
        {
            get { return _categoryNamePrefix; }
        }

        private String _machineName;
        public String GetMachineNameVal()
        {
            return _machineName;
        }
        public void SetMachineNameVal(String value)
        {
            _machineName = value;
        }
        String IWinCounters.MachineName
        {
            get { return _machineName; }
        }

        private Boolean _isReadOnlyCounters;
        public Boolean GetIsReadOnlyCountersVal()
        {
            return _isReadOnlyCounters;
        }
        public void SetIsReadOnlyCountersVal(Boolean value)
        {
            _isReadOnlyCounters = value;
        }
        Boolean IWinCounters.IsReadOnlyCounters
        {
            get { return _isReadOnlyCounters; }
        }

        private WinCountersPreferedBitnessCfg _preferedBitness;
        public WinCountersPreferedBitnessCfg GetPreferedBitnessVal()
        {
            return _preferedBitness;
        }
        public void SetPreferedBitnessVal(WinCountersPreferedBitnessCfg value)
        {
            _preferedBitness = value;
        }
        WinCountersPreferedBitnessCfg IWinCounters.PreferedBitness
        {
            get { return _preferedBitness; }
        }

        private WinCountersExistedInstancesTreatmentCfg _existedInstancesTreatment;
        public WinCountersExistedInstancesTreatmentCfg GetExistedInstancesTreatmentVal()
        {
            return _existedInstancesTreatment;
        }
        public void SetExistedInstancesTreatmentVal(WinCountersExistedInstancesTreatmentCfg value)
        {
            _existedInstancesTreatment = value;
        }
        WinCountersExistedInstancesTreatmentCfg IWinCounters.ExistedInstancesTreatment
        {
            get { return _existedInstancesTreatment; }
        }


        public WinCountersImplement Copy()
        {
            var res = new WinCountersImplement();

            res._instantiationMode = this._instantiationMode;
            res._categoryNamePrefix = this._categoryNamePrefix;
            res._machineName = this._machineName;
            res._isReadOnlyCounters = this._isReadOnlyCounters;
            res._preferedBitness = this._preferedBitness;
            res._existedInstancesTreatment = this._existedInstancesTreatment;

            return res;
        }

        public static IWinCounters CopyInh(IWinCounters src)
        {
            if (src == null)
                return null;

            if (src.GetType() == typeof(WinCountersImplement))
                return ((WinCountersImplement)src).Copy();
            if (src.GetType() == typeof(WinCountersImplement))
                return ((WinCountersImplement)src).Copy();

            throw new Exception("Unknown type: " + src.GetType().ToString());
        }
    }


    // ============================

    internal class PerfCountersConfigurationSectionConfigClass : System.Configuration.ConfigurationSection
    {
    	private IPerfCountersConfigurationSection _configData = new PerfCountersConfigurationSectionImplement();
    	public IPerfCountersConfigurationSection ConfigData { get {return _configData; } }
     
    
    	public IPerfCountersConfigurationSection ExtractConfigData()
    	{
    		return PerfCountersConfigurationSectionImplement.CopyInh(_configData);
    	}
    
    	protected override void InitializeDefault()
        {
    		base.InitializeDefault();
            _configData = new PerfCountersConfigurationSectionImplement();
        }
    
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
    		if (reader.NodeType == System.Xml.XmlNodeType.None)
    			reader.Read();
    		_configData = DeserializeIPerfCountersConfigurationSectionElem(reader);
        }
    
        public override bool IsReadOnly()
        {
    		return true;
        }
    	
    
    	private T Parse<T>(string value)
        {
    		return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }
    
    
        private List<T> DeserializeList<T>(System.Xml.XmlReader reader, Func<System.Xml.XmlReader, T> readFnc, string expectedName)
        {
    	    if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
    
            List<T> res = new List<T>();
        
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return res;
            }
        
            string initialName = reader.Name;
        
            reader.ReadStartElement();
        
            do
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    if (expectedName != null && reader.Name != expectedName)
                        throw new System.Configuration.ConfigurationErrorsException("Unexpected element name inside list: " + reader.Name, reader);
    
                    T elem = readFnc(reader);
                    res.Add(elem);
                }
                else
                {
                    reader.Skip();
                }
            }
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
        
        	reader.ReadEndElement();
        
            return res;
        }
    
    
        private Dictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(System.Xml.XmlReader reader, Func<System.Xml.XmlReader, TValue> readFnc, string expectedName)
        {
    	    if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
    
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
        
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return res;
            }
        
            string initialName = reader.Name;
        
            reader.ReadStartElement();
        
            do
            {
                if (reader.NodeType == System.Xml.XmlNodeType.Element)
                {
                    if (expectedName != null && reader.Name != expectedName)
                        throw new System.Configuration.ConfigurationErrorsException("Unexpected element name inside list: " + reader.Name, reader);
    
                    string strKey = reader.GetAttribute("key");
                    if (strKey == null)
                        throw new System.Configuration.ConfigurationErrorsException("Key not found for dictionary: " + reader.Name, reader);
        
                    TKey key = Parse<TKey>(strKey);
                    TValue val = readFnc(reader);
        
                    res.Add(key, val);
                }
                else
                {
                    reader.Skip();
                }
            }
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
        
        	reader.ReadEndElement();
        
            return res;
        }
        
        
        private T DeserializeSimpleValueElement<T>(System.Xml.XmlReader reader)
        {
    		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
    
            string ElemName = reader.Name;
        
            string addValue = reader.GetAttribute("value");
            if (addValue == null)
                throw new System.Configuration.ConfigurationErrorsException("Value not found for SimpleValueElement '" + ElemName + "' inside element", reader);
        
            T res = Parse<T>(addValue);
        
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                if (reader.MoveToContent() != System.Xml.XmlNodeType.EndElement)
                    throw new System.Configuration.ConfigurationErrorsException("SimpleValueElement '" + ElemName + "' can't contains any other elements", reader);
                reader.ReadEndElement();
            }
        
            return res;
        }
    
    
        private T DeserializeSimpleValueElement<T>(System.Xml.XmlReader reader, string expectedName)
        {
    	    if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
    
            if (expectedName != null && reader.Name != expectedName)
                throw new System.Configuration.ConfigurationErrorsException("Unexpected element name inside list: " + reader.Name, reader);
    
            string ElemName = reader.Name;
    
            string addValue = reader.GetAttribute("value");
            if (addValue == null)
                throw new System.Configuration.ConfigurationErrorsException("Value not found for SimpleValueElement '" + ElemName + "' inside element", reader);
    
            T res = Parse<T>(addValue);
    
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                if (reader.MoveToContent() != System.Xml.XmlNodeType.EndElement)
                    throw new System.Configuration.ConfigurationErrorsException("SimpleValueElement '" + ElemName + "' can't contains any other elements", reader);
                reader.ReadEndElement();
            }
    
            return res;
        }
    
    
    
        private IPerfCountersConfigurationSection DeserializeIPerfCountersConfigurationSectionElem(System.Xml.XmlReader reader)
        {
        	var res = new PerfCountersConfigurationSectionImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        

            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        reader.Read();
                        if (reader.MoveToContent() != System.Xml.XmlNodeType.Element)
                            throw new System.Configuration.ConfigurationErrorsException("Injected element not found for property 'rootCounters' inside 'IPerfCountersConfigurationSection", reader);
                        res.SetRootCountersVal(DeserializeIPerfCountersConfigurationElemWithInh(reader));
                        if (reader.MoveToContent() != System.Xml.XmlNodeType.EndElement)
                            throw new System.Configuration.ConfigurationErrorsException("Injected element 'rootCounters' inside 'IPerfCountersConfigurationSection can't contain several subelements", reader);
                        reader.ReadEndElement();
                        parsedElements.Add("rootCounters");
                        break;
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.Add("rootCounters");
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private IPerfCountersConfigurationSection DeserializeIPerfCountersConfigurationSectionElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'IPerfCountersConfigurationSection': " + reader.Name, reader);
            
        	return DeserializeIPerfCountersConfigurationSectionElem(reader);
        }

        private IPerfCountersConfigurationSection DeserializeIPerfCountersConfigurationSectionElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "perfCountersConfigurationSection":
                    return DeserializeIPerfCountersConfigurationSectionElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'IPerfCountersConfigurationSection'", reader);
            }
        }


        private IPerfCountersConfiguration DeserializeIPerfCountersConfigurationElem(System.Xml.XmlReader reader)
        {
        	var res = new PerfCountersConfigurationImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        

            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
                reader.ReadStartElement();
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                	throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'IPerfCountersConfiguration'", reader);	
                                
                                switch (addKey)
                                {
                                	default:
                                		throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'IPerfCountersConfiguration'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'IPerfCountersConfiguration': " + reader.Name, reader);
                        }
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
                reader.ReadEndElement();
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private IPerfCountersConfiguration DeserializeIPerfCountersConfigurationElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'IPerfCountersConfiguration': " + reader.Name, reader);
            
        	return DeserializeIPerfCountersConfigurationElem(reader);
        }

        private IPerfCountersConfiguration DeserializeIPerfCountersConfigurationElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "compositeCounters":
                    return DeserializeICompositeCountersElem(reader);
                case "netCounters":
                    return DeserializeINetCountersElem(reader);
                case "graphiteCounters":
                    return DeserializeIGraphiteCountersElem(reader);
                case "internalCounters":
                    return DeserializeIInternalCountersElem(reader);
                case "nullCounters":
                    return DeserializeINullCountersElem(reader);
                case "winCounters":
                    return DeserializeIWinCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'IPerfCountersConfiguration'", reader);
            }
        }


        private ICompositeCounters DeserializeICompositeCountersElem(System.Xml.XmlReader reader)
        {
        	var res = new CompositeCountersImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        

            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        var tmp_WrappedCounters = DeserializeList(reader, DeserializeIPerfCountersConfigurationElemWithInh, null);
                        res.SetWrappedCountersVal(tmp_WrappedCounters);
                        break;
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private ICompositeCounters DeserializeICompositeCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'ICompositeCounters': " + reader.Name, reader);
            
        	return DeserializeICompositeCountersElem(reader);
        }

        private ICompositeCounters DeserializeICompositeCountersElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "compositeCounters":
                    return DeserializeICompositeCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'ICompositeCounters'", reader);
            }
        }


        private INetCounters DeserializeINetCountersElem(System.Xml.XmlReader reader)
        {
        	var res = new NetCountersImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        
            string attribGenTempVal = null;
            attribGenTempVal = reader.GetAttribute("distributionPeriodMs");
            if (attribGenTempVal != null)
                res.SetDistributionPeriodMsVal(Parse<Int32>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("serverAddress");
            if (attribGenTempVal != null)
                res.SetServerAddressVal(Parse<String>(attribGenTempVal));
            else
                throw new System.Configuration.ConfigurationErrorsException("Attribute 'serverAddress for element 'INetCounters' not defined", reader);

            attribGenTempVal = reader.GetAttribute("serverPort");
            if (attribGenTempVal != null)
                res.SetServerPortVal(Parse<Int32>(attribGenTempVal));


            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
                reader.ReadStartElement();
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                	throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'INetCounters'", reader);	
                                
                                switch (addKey)
                                {
                                	default:
                                		throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'INetCounters'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'INetCounters': " + reader.Name, reader);
                        }
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
                reader.ReadEndElement();
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private INetCounters DeserializeINetCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'INetCounters': " + reader.Name, reader);
            
        	return DeserializeINetCountersElem(reader);
        }

        private INetCounters DeserializeINetCountersElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "netCounters":
                    return DeserializeINetCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'INetCounters'", reader);
            }
        }


        private IGraphiteCounters DeserializeIGraphiteCountersElem(System.Xml.XmlReader reader)
        {
        	var res = new GraphiteCountersImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        
            string attribGenTempVal = null;
            attribGenTempVal = reader.GetAttribute("distributionPeriodMs");
            if (attribGenTempVal != null)
                res.SetDistributionPeriodMsVal(Parse<Int32>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("namePrefixFormatString");
            if (attribGenTempVal != null)
                res.SetNamePrefixFormatStringVal(Parse<String>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("serverAddress");
            if (attribGenTempVal != null)
                res.SetServerAddressVal(Parse<String>(attribGenTempVal));
            else
                throw new System.Configuration.ConfigurationErrorsException("Attribute 'serverAddress for element 'IGraphiteCounters' not defined", reader);

            attribGenTempVal = reader.GetAttribute("serverPort");
            if (attribGenTempVal != null)
                res.SetServerPortVal(Parse<Int32>(attribGenTempVal));


            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
                reader.ReadStartElement();
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                	throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'IGraphiteCounters'", reader);	
                                
                                switch (addKey)
                                {
                                	default:
                                		throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'IGraphiteCounters'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'IGraphiteCounters': " + reader.Name, reader);
                        }
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
                reader.ReadEndElement();
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private IGraphiteCounters DeserializeIGraphiteCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'IGraphiteCounters': " + reader.Name, reader);
            
        	return DeserializeIGraphiteCountersElem(reader);
        }

        private IGraphiteCounters DeserializeIGraphiteCountersElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "graphiteCounters":
                    return DeserializeIGraphiteCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'IGraphiteCounters'", reader);
            }
        }


        private IInternalCounters DeserializeIInternalCountersElem(System.Xml.XmlReader reader)
        {
        	var res = new InternalCountersImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        

            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
                reader.ReadStartElement();
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                	throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'IInternalCounters'", reader);	
                                
                                switch (addKey)
                                {
                                	default:
                                		throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'IInternalCounters'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'IInternalCounters': " + reader.Name, reader);
                        }
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
                reader.ReadEndElement();
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private IInternalCounters DeserializeIInternalCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'IInternalCounters': " + reader.Name, reader);
            
        	return DeserializeIInternalCountersElem(reader);
        }

        private IInternalCounters DeserializeIInternalCountersElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "internalCounters":
                    return DeserializeIInternalCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'IInternalCounters'", reader);
            }
        }


        private INullCounters DeserializeINullCountersElem(System.Xml.XmlReader reader)
        {
        	var res = new NullCountersImplement();
        
        	HashSet<string> parsedElements = new HashSet<string>();
        

            if (reader.IsEmptyElement)
            {
            	reader.Skip();
            }
            else
            {
            	string initialName = reader.Name;
                reader.ReadStartElement();
            	do
                {
            		if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
            			reader.Skip();
            		}
            		else
            		{			
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                	throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'INullCounters'", reader);	
                                
                                switch (addKey)
                                {
                                	default:
                                		throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'INullCounters'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'INullCounters': " + reader.Name, reader);
                        }
            		}
            	}
            	while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);
            
                reader.ReadEndElement();
            }
            
            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ",restElems));
            return res;
        }

        private INullCounters DeserializeINullCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);
        
        	if (expectedName != null && reader.Name != expectedName)
        		throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'INullCounters': " + reader.Name, reader);
            
        	return DeserializeINullCountersElem(reader);
        }

        private INullCounters DeserializeINullCountersElemWithInh(System.Xml.XmlReader reader)
        {
        	if (reader.NodeType != System.Xml.XmlNodeType.Element)
        		throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);
        
        	switch (reader.Name)
        	{
                case "nullCounters":
                    return DeserializeINullCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'INullCounters'", reader);
            }
        }


        private IWinCounters DeserializeIWinCountersElem(System.Xml.XmlReader reader)
        {
            var res = new WinCountersImplement();

            HashSet<string> parsedElements = new HashSet<string>();

            string attribGenTempVal = null;
            attribGenTempVal = reader.GetAttribute("instantiationMode");
            if (attribGenTempVal != null)
                res.SetInstantiationModeVal(Parse<WinCountersInstantiationModeCfg>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("categoryNamePrefix");
            if (attribGenTempVal != null)
                res.SetCategoryNamePrefixVal(Parse<String>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("machineName");
            if (attribGenTempVal != null)
                res.SetMachineNameVal(Parse<String>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("isReadOnlyCounters");
            if (attribGenTempVal != null)
                res.SetIsReadOnlyCountersVal(Parse<Boolean>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("preferedBitness");
            if (attribGenTempVal != null)
                res.SetPreferedBitnessVal(Parse<WinCountersPreferedBitnessCfg>(attribGenTempVal));

            attribGenTempVal = reader.GetAttribute("existedInstancesTreatment");
            if (attribGenTempVal != null)
                res.SetExistedInstancesTreatmentVal(Parse<WinCountersExistedInstancesTreatmentCfg>(attribGenTempVal));


            if (reader.IsEmptyElement)
            {
                reader.Skip();
            }
            else
            {
                string initialName = reader.Name;
                reader.ReadStartElement();
                do
                {
                    if (reader.NodeType != System.Xml.XmlNodeType.Element)
                    {
                        reader.Skip();
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "add":
                                string addKey = reader.GetAttribute("key");
                                if (addKey == null)
                                    throw new System.Configuration.ConfigurationErrorsException("Key not found for 'add' inside element 'IWinCounters'", reader);

                                switch (addKey)
                                {
                                    default:
                                        throw new System.Configuration.ConfigurationErrorsException("Unknown key " + addKey + " inside element 'IWinCounters'", reader);
                                }
                            default:
                                throw new System.Configuration.ConfigurationErrorsException("Unknown element inside 'IWinCounters': " + reader.Name, reader);
                        }
                    }
                }
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != initialName);

                reader.ReadEndElement();
            }

            HashSet<string> restElems = new HashSet<string>();
            restElems.RemoveWhere(o => parsedElements.Contains(o));
            if (restElems.Count > 0)
                throw new System.Configuration.ConfigurationErrorsException("Not all required properties readed: " + string.Join(", ", restElems));
            return res;
        }

        private IWinCounters DeserializeIWinCountersElem(System.Xml.XmlReader reader, string expectedName)
        {
            if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Expected Element node type", reader);

            if (expectedName != null && reader.Name != expectedName)
                throw new System.Configuration.ConfigurationErrorsException("Unexpected element name for type 'IWinCounters': " + reader.Name, reader);

            return DeserializeIWinCountersElem(reader);
        }

        private IWinCounters DeserializeIWinCountersElemWithInh(System.Xml.XmlReader reader)
        {
            if (reader.NodeType != System.Xml.XmlNodeType.Element)
                throw new System.Configuration.ConfigurationErrorsException("Not an Element node type", reader);

            switch (reader.Name)
            {
                case "winCounters":
                    return DeserializeIWinCountersElem(reader);
                default:
                    throw new System.Configuration.ConfigurationErrorsException("Unknown child type name: '" + reader.Name + "' for base type 'IWinCounters'", reader);
            }
        }


    }


}
