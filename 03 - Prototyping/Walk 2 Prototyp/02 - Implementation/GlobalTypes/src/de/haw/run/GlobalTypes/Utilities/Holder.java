package de.haw.run.GlobalTypes.Utilities;

class Holder<T> {
    private T value;

    Holder(T value) {
        setValue(value);
    }

    T getValue() {
        return value;
    }

    void setValue(T value) {
        this.value = value;
    }
}