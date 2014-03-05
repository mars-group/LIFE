package forest;


public class Tree {

    private int age;
    private int height;
    private int carbonAmount;
    private int ID;


    public Tree(int age, int height, int carbonAmount, int id) {
        this.ID = id;
        this.age = age;
        this.height = height;
        this.carbonAmount = carbonAmount;
    }

    public int getHeight() {
        return height;
    }

    public int getCarbonAmount() {
        return carbonAmount;
    }

    public int getAge() {
        return age;
    }

    public int getID() {
        return ID;
    }
}
