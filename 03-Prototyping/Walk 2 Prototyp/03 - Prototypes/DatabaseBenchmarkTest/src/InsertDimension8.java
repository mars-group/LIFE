import org.voltdb.SQLStmt;
import org.voltdb.VoltProcedure;
import org.voltdb.VoltTable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 28.06.13
 * Time: 12:22
 */
public class InsertDimension8 extends VoltProcedure {
    public final SQLStmt sql = new SQLStmt(
            "INSERT INTO d_t8 VALUES (?, ? ,?);"
    );

    public VoltTable[] run(String value, long time, String functionalId){
        voltQueueSQL(sql,value,time,functionalId);
        voltExecuteSQL();
        return null;
    }
}
