namespace Senzing.Sdk.Tests.Core;

using System.Text;
using static Senzing.Sdk.Utilities;
using static Senzing.Sdk.Tests.Core.AbstractTest;

public class SzRecord {
    public interface SzRecordData {
        string GetPluralName() {
            return "";
        }
    }

    public record SzDataSourceCode(string DataSourceCode) : SzRecordData {
        public static SzDataSourceCode Of(string DataSourceCode) {
            return new SzDataSourceCode(DataSourceCode);
        }
        public override string ToString() {
            return "\"DATA_SOURCE\":" + JsonEscape(this.DataSourceCode);
        }
    }

    public record SzRecordID(string RecordID) : SzRecordData {
        public static SzRecordID Of(string recordID) {
            return new SzRecordID(recordID);
        }
        public override string ToString() {
            return "\"RECORD_ID\":" + JsonEscape(this.RecordID);
        }
    }

    public record SzRecordType(string RecordType) : SzRecordData {
        public static SzRecordType Of(string recordType) {
            return new SzRecordType(recordType);
        }
        public override string ToString() {
            return "\"RECORD_TYPE\":" + JsonEscape(this.RecordType);
        }
    }

    public interface SzName : SzRecordData {
        string? NameType { get; init; }

        new string GetPluralName() {
            return "NAMES";
        }
    }

    public interface SzAddress : SzRecordData {
        string? AddressType { get; init; }
        
        new string GetPluralName() {
            return "ADDRESSES";
        }
    }

    public record SzFullName(string FullName, string? NameType) : SzName
    {   
        public static SzFullName Of(string fullName) {
            return new SzFullName(fullName, null);
        }
        public static SzFullName Of(string fullName, string? nameType) {
            return new SzFullName(fullName, nameType);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"NAME_FULL\":" + JsonEscape(this.FullName));
            if (this.NameType != null) {
                sb.Append(",\"NAME_TYPE\":" + JsonEscape(this.NameType));
            }
            return sb.ToString();
        }    
    }

    public record SzNameByParts(
        string? FirstName, string? LastName, string? NameType) : SzName
    {
        public static SzNameByParts Of(string? firstName, string? lastName) {
            return new SzNameByParts(firstName, lastName, null);
        }
        public static SzNameByParts Of(
            string? firstName, string? lastName, string? nameType)
        {
            return new SzNameByParts(firstName, lastName, nameType);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            string prefix = "";
            if (this.FirstName != null) {
                sb.Append("\"NAME_FIRST\":" + JsonEscape(this.FirstName));
                prefix = ",";
            }
            if (this.LastName != null) {
                sb.Append(prefix);
                sb.Append("\"NAME_LAST\":" + JsonEscape(this.LastName));
                prefix = ",";
            }
            if (this.NameType != null) {
                sb.Append(prefix);
                sb.Append("\"NAME_TYPE\":" + JsonEscape(this.NameType));
            }
            return sb.ToString();
        }    
    }

    public record SzFullAddress(string FullAddress, string? AddressType) : SzAddress
    {
        public static SzFullAddress Of(string fullAddress) {
            return new SzFullAddress(fullAddress, null);
        }
        public static SzFullAddress Of(string fullAddress, string? addressType) {
            return new SzFullAddress(fullAddress, addressType);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"ADDR_FULL\":" + JsonEscape(this.FullAddress));
            if (this.AddressType != null) {
                sb.Append(",\"ADDR_TYPE\":" + JsonEscape(this.AddressType));
            }
            return sb.ToString();
        }    
    }

    public record SzAddressByParts(
        string? Street, string? City, string? State, string? PostalCode, string? AddressType) : SzAddress
    {
        public static SzAddressByParts Of(string? street, string? city, string? state, string? postalCode) {
            return new SzAddressByParts(street, city, state, postalCode, null);
        }
        public static SzAddressByParts Of(string? street, string? city, string? state, string? postalCode, string? addressType) {
            return new SzAddressByParts(street, city, state, postalCode, addressType);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            string prefix = "";
            if (this.Street != null) {
                sb.Append("\"ADDR_LINE1\":" + JsonEscape(this.Street));
                prefix = ",";
            }
            if (this.City != null) {
                sb.Append(prefix);
                sb.Append("\"ADDR_CITY\":" + JsonEscape(this.City));
                prefix = ",";
            }
            if (this.State != null) {
                sb.Append(prefix);
                sb.Append("\"ADDR_STATE\":" + JsonEscape(this.State));
            }
            if (this.PostalCode != null) {
                sb.Append(prefix);
                sb.Append("\"ADDR_POSTAL_CODE\":" + JsonEscape(this.PostalCode));
            }
            if (this.AddressType != null) {
                sb.Append(prefix);
                sb.Append("\"ADDR_TYPE\":" + JsonEscape(this.AddressType));
            }
            return sb.ToString();
        }            

    }

    public record SzPhoneNumber(string Phone, string? PhoneType) : SzRecordData
    {
        public string GetPluralName() {
            return "PHONE_NUMBERS";
        }

        public static SzPhoneNumber Of(string phone) {
            return new SzPhoneNumber(phone, null);
        }
        public static SzPhoneNumber Of(string phone, string? phoneType) {
            return new SzPhoneNumber(phone, phoneType);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"PHONE_MUMBER\":" + JsonEscape(this.Phone));
            if (this.PhoneType != null) {
                sb.Append(",\"PHONE_TYPE\":" + JsonEscape(this.PhoneType));
            }
            return sb.ToString();
        }    
    }

    public record SzEmailAddress(string Email, string? EmailType) : SzRecordData
    {
        public string GetPluralName() {
            return "EMAILS";
        }
        public static SzEmailAddress Of(string email) {
            return new SzEmailAddress(email, null);
        }
        public static SzEmailAddress Of(string email, string emailType) {
            return new SzEmailAddress(email, emailType);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"EMAIL_ADDRESS\":" + JsonEscape(this.Email));
            if (this.EmailType != null) {
                sb.Append(",\"EMAIL_TYPE\":" + JsonEscape(this.EmailType));
            }
            return sb.ToString();
        }    
    }

    public record SzDateOfBirth(string Date) :  SzRecordData {
        public static SzDateOfBirth Of(string date) {
            return new SzDateOfBirth(date);
        }
        public override string ToString () {
            StringBuilder sb = new StringBuilder();
            sb.Append ("\"DATE_OF_BIRTH\":" + JsonEscape(this.Date));
            return sb.ToString();
        }
    }
    
    public record SzSocialSecurity(string Ssn) :  SzRecordData {
        public static SzSocialSecurity Of(string ssn) {
            return new SzSocialSecurity(ssn);
        }
    
        public override string ToString () {
            StringBuilder sb = new StringBuilder();
            sb.Append ("\"SSN_NUMBER\":" + JsonEscape(this.Ssn));
            return sb.ToString();
        }    
    }

    public record SzDriversLicense(string LicenseNumber, string? State)
        :  SzRecordData
    {
        public static SzDriversLicense Of(string licenseNumber) {
            return new SzDriversLicense(licenseNumber, null);
        }
        public static SzDriversLicense Of(string licenseNumber, string? state) {
            return new SzDriversLicense(licenseNumber, state);
        }
        public override string ToString () {
            StringBuilder sb = new StringBuilder();
            sb.Append ("\"DRIVERS_LICENSE_NUMBER\":" + JsonEscape(this.LicenseNumber));
            if (this.State != null) {
                sb.Append (",\"DRIVERS_LICENSE_STATE\":" + JsonEscape(this.State));
            }
            return sb.ToString();
        }    
    }

    public record SzPassport(string PassportNumber, string? Country)
        :  SzRecordData
    {
        public static SzPassport Of(string passportNumber) {
            return new SzPassport(passportNumber, null);
        }
        public static SzPassport Of(string passportNumber, string country) {
            return new SzPassport(passportNumber, country);
        }
        public override string ToString () {
            StringBuilder sb = new StringBuilder();
            sb.Append ("\"PASSPORT_NUMBER\":" + JsonEscape(this.PassportNumber));
            if (this.Country != null) {
                sb.Append (",\"PASSPORT_COUNTRY\":" + JsonEscape(this.Country));
            }
            return sb.ToString();
        }    
    }

    private (string dataSourceCode, string recordID)? recordKey;

    private (string dataSourceCode, string recordID)? RecordKey { get {
        return this.recordKey;
    }}

    private IDictionary<object, List<SzRecordData>> dataMap;
    private List<object> dataTypeList;

    public SzRecord((string dataSourceCode, string recordID)? recordKey, SzRecord other) {
        this.recordKey = recordKey;
        this.dataMap        = new Dictionary<object, List<SzRecordData>>();
        this.dataTypeList   = new List<object>();

        if (this.recordKey != null) {
            this.dataTypeList.Add(typeof(SzDataSourceCode));
            this.dataMap.Add(typeof(SzDataSourceCode),
                ListOf<SzRecordData>(SzDataSourceCode.Of(recordKey?.dataSourceCode ?? "")));

            this.dataTypeList.Add(typeof(SzRecordID));
            this.dataMap.Add(typeof(SzRecordID),
                ListOf<SzRecordData>(SzRecordID.Of(recordKey?.recordID ?? "")));
        }
        if (other != null) {
            foreach (object key in other.dataTypeList) {
                if (key.Equals(typeof(SzDataSourceCode))) continue;
                if (key.Equals(typeof(SzRecordID))) continue;
                this.dataTypeList.Add(key);
                this.dataMap.Add(key, other.dataMap[key]);
            }
        }
    }

    public SzRecord((string dataSourceCode, string recordID)?   recordKey, 
                     params SzRecordData[]                      recordData) 
        : this(recordData)
    {
        this.recordKey = recordKey;
        this.dataMap.Remove(typeof(SzDataSourceCode));
        this.dataMap.Remove(typeof(SzRecordID));
        this.dataTypeList.Remove(typeof(SzDataSourceCode));
        this.dataTypeList.Remove(typeof(SzRecordID));

        if (recordKey != null) {
            this.dataTypeList.Insert(0, typeof(SzRecordID));
            this.dataTypeList.Insert(0, typeof(SzDataSourceCode));

            this.dataMap.Add(typeof(SzDataSourceCode), 
                ListOf<SzRecordData>(SzDataSourceCode.Of(recordKey?.dataSourceCode ?? "")));
            this.dataMap.Add(typeof(SzRecordID),
                ListOf<SzRecordData>(SzRecordID.Of(recordKey?.recordID ?? "")));
        }
    }

    public SzRecord(params SzRecordData[] recordData) {
        this.dataMap = new Dictionary<object,List<SzRecordData>>();
        this.dataTypeList = new List<object>();

        string? dataSourceCode   = null;
        string? recordID         = null;

        if (recordData != null) {
            foreach (SzRecordData data in recordData) {
                if (data == null) continue;
                if (data is SzDataSourceCode) {
                    dataSourceCode = ((SzDataSourceCode) data).DataSourceCode;
                }
                if (data is SzRecordID) {
                    recordID = ((SzRecordID) data).RecordID;
                }
                Type classKey = data.GetType();
                string pluralName = data.GetPluralName();
                object key = ((pluralName.Length == 0) ? ((object) classKey) : ((object) pluralName));
                if (!this.dataMap.ContainsKey(key)) {;
                    this.dataMap.Add(key, new List<SzRecordData>());
                    this.dataTypeList.Add(key);
                }
                List<SzRecordData> list = this.dataMap[key];
                if (list.Count > 0 && pluralName == null) {
                    throw new ArgumentException(
                        "Multiple values for " + classKey.Name 
                        + " are not allowed.  specified=[ " + data 
                        + "], existing=[ " + list[0] + " ]");
                }
                list.Add(data);
            }
        }
        if (dataSourceCode != null && recordID != null) {
            this.recordKey = (dataSourceCode, recordID);
        }
    }

    public (string dataSourceCode, string recordID)? GetRecordKey() {
        return this.recordKey;
    }

    public override string ToString () {
        StringBuilder sb = new StringBuilder();
        sb.Append ("{");
        string prefix = "";
        foreach (object key in this.dataTypeList) {
            List<SzRecordData> valueList = this.dataMap[key];
            sb.Append (prefix);
            if (valueList.Count == 1) {
                sb.Append(valueList[0]);
            } else {
                string pluralName = valueList[0].GetPluralName();
                sb.Append(JsonEscape(pluralName)).Append (":[");
                string subPrefix = "";
                foreach (SzRecordData data in valueList) {
                    sb.Append(subPrefix);
                    sb.Append("{").Append(data).Append("}");
                    subPrefix = ",";
                }
                sb.Append("]");
            }
            prefix = ",";
        }
        sb.Append("}");

        return sb.ToString();
    }
}
