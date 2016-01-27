import org.voltdb.SQLStmt;
import org.voltdb.VoltProcedure;
import org.voltdb.VoltSystemProcedure;
import org.voltdb.VoltTable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 28.06.13
 * Time: 12:21
 */
public class InsertDimension5 extends VoltProcedure {
    public final SQLStmt sql = new SQLStmt(
            "INSERT INTO d_t5 VALUES (?, ? ,?);"
    );

    public VoltTable[] run(String value, long time, String functionalId){
        voltQueueSQL(sql,value,time,functionalId);
        voltExecuteSQL();
        return null;
    }
}