
using System.Collections.Generic;

public class Bound<T> {
	private T _value;
	public T value { 
		get {return _value; } 
		set { Set(value); }
	}
	public Option<T> max;
	public Option<T> min;

	public Bound(T value, T max) : this(value, Option<T>.None, new Option<T>(max)) {}

	public Bound(T value, T min, T max) : this(value, new Option<T>(min), new Option<T>(max)) {}

	public Bound(T value, Option<T> min, Option<T> max) {
		this.min = min;
		this.max = max;
		Set(value);
	}

	public void Set(T value) {
		if (min.Get(out T minValue)) {
			dynamic min = minValue;
			dynamic val = value;
			if (min > val)
				value = minValue;
		}
		
		if (max.Get(out T maxValue)) {
			dynamic max = maxValue;
			dynamic val = value;
			if (max < val)
				value = maxValue;
		}
		this._value = value;
	}

	public void Set(Option<T> value) {
		if (value.Get(out T boundValue))
			Set(boundValue);
	}

	public void SetToMax() => Set(max);
	public void SetToMin() => Set(min);

	public Bound<T> Add(T value) {
		dynamic self = this.value;
		dynamic val = value;
		Set(self + value);
		return this;
	}

	public Bound<T> Sub(T value) {
		dynamic self = this.value;
		dynamic val = value;
		Set(self - value);
		return this;
	}

	public override bool Equals(object obj)
	{
		return obj is Bound<T> bound &&
			   EqualityComparer<T>.Default.Equals(_value, bound._value);
	}

	public override int GetHashCode()
	{
		return -1939223833 + EqualityComparer<T>.Default.GetHashCode(_value);
	}

	public bool isMax {
		get {
			if (max.Get(out T maxValue)) {
				dynamic val = value;
				dynamic max = maxValue;
				return val >= max;
			}
			return false;
		}
	}

	public bool isMin {
		get {
			if (min.Get(out T minValue)) {
				dynamic val = value;
				dynamic min = minValue;
				return val <= min;
			}
			return false;
		}
	}

	public static Bound<T> operator +(Bound<T> left, T right) => left.Add(right);
	public static Bound<T> operator -(Bound<T> left, T right) => left.Sub(right);

	public static bool operator ==(Bound<T> left, T right) => left.value.Equals(right);
	public static bool operator !=(Bound<T> left, T right) => !left.value.Equals(right);

	public static bool operator <(Bound<T> left, T right) {
		dynamic l = left.value;
		dynamic r = right;
		return l < r;
	}
	
	public static bool operator >(Bound<T> left, T right) {
		dynamic l = left.value;
		dynamic r = right;
		return l > r;
	}


	public static bool operator <=(Bound<T> left, T right) {
		dynamic l = left.value;
		dynamic r = right;
		return l <= r;
	}
	
	public static bool operator >=(Bound<T> left, T right) {
		dynamic l = left.value;
		dynamic r = right;
		return l >= r;
	}

	public override string ToString() {
		var minStr = min.Get(out T minValue) ? $"{minValue}/" : "";
		var maxStr = max.Get(out T maxValue) ? $"/{maxValue}" : "";
		return $"{minStr}{value}{maxStr}";
	}
}