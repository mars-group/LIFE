import org.voltdb.SQLStmt;
import org.voltdb.VoltProcedure;
import org.voltdb.VoltTable;

public class Insert extends VoltProcedure {


    public final SQLStmt sql = new SQLStmt(
            "INSERT INTO HELOWORLD VALUE (?, ? ,?);"
    );

    public VoltTable[] run(String language, String hello, String world){
        voltQueueSQL(sql,hello,world,language);
        voltExecuteSQL();
        return null;
    }
}
