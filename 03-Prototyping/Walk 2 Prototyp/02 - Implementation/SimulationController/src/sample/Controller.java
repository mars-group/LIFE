package sample;

import javafx.event.ActionEvent;
import javafx.scene.control.Button;
import javafx.scene.control.Label;

public class Controller {
    public Label label1;
    public Button button1;

    public void onButtonAction(ActionEvent actionEvent) {
        label1.setText("Hallo Welt");
    }
}
