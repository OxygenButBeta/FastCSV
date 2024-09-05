
namespace o2.FastCSV
{
    public class DataCell
    {
        /// Fields
        private object _value;
        private bool _parentTableExist;

        /// Properties
        public DataRow RelatedRow { get; private set; }
        public object Value
        {
            get => Convert.ChangeType(_value, CellDataType) as object;
            set
            {
                string oldValue = ToString();
                _value = value;
                CellDataType = DefineType(value.ToString());
                if (RelatedRow.ParentTable.CaptureChanges)
                    RelatedRow.ParentTable.ChangeLogs.Add($"{oldValue} has been changed to {ToString()}");

            }
        }
        public Type CellDataType { get; private set; }

        /// <summary>
        /// Get the position of the cell in the table
        /// </summary>
        public CellPosition Position { get; private set; }



        /// Constructors
        public DataCell(object Value, CellPosition Position, DataRow ParentRow)
        {
            this._value = Value;
            this.CellDataType = DefineType(Value.ToString());
            this.Position = Position;
            this.RelatedRow = ParentRow;
            _parentTableExist = ParentRow.ParentTable is not null;
        }

        /// Define the type of the value
        Type DefineType(string input)
        {
            return input switch
            {
                _ when bool.TryParse(input, out bool _) => typeof(bool),
                _ when int.TryParse(input, out int _) => typeof(int),
                _ when long.TryParse(input, out long _) => typeof(long),
                _ when float.TryParse(input, out float _) => typeof(float),
                _ when double.TryParse(input, out double _) => typeof(double),
                _ when decimal.TryParse(input, out decimal _) => typeof(decimal),
                _ => typeof(string) // return string if no other type matches
            };
        }

        public override bool Equals(object obj)
        {


            if (!_parentTableExist)
            {
                if (!RelatedRow.ParentTable.CheckCellEqualityAsString)
                {
                    if (this.CellDataType == obj.GetType())
                        return Value.Equals(obj);
                    else if (obj is DataCell)
                        return (obj as DataCell).Value == this.Value;
                }
                else
                {
                    return Value.ToString() == obj.ToString();
                }
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => base.GetHashCode();
        public override string ToString()
        {
            var typeAsString = CellDataType.ToString().Split('.');
            return $"[{Position}, Value: {Value}, Data Type: {typeAsString[typeAsString.Length - 1]}]";
        }
    }
}
