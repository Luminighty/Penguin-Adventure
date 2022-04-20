public class Option<T> {
	private bool hasValue = false;
	private T value;

	public bool isSome { get {
		return hasValue;
	}}

	public bool isNone { get {
		return !isSome;
	}}

	public static Option<T> None {
		get {
			return new Option<T>();
		}
	}

	public Option() {
		this.hasValue = false;
	}

	public Option(T value) {
		this.hasValue = true;
		this.value = value;
	}

	public void Set(T value) {
		hasValue = value != null;
		this.value = value;
	}

	public bool Get(out T value) {
		value = this.value;
		return hasValue;
	}
}