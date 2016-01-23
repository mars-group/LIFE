import org.voltdb.ProcInfo;
import org.voltdb.SQLStmt;
import org.voltdb.VoltTable;
import org.voltdb.VoltProcedure;

/**
 * Project: RUN
 * User: chhuening
 * Date: 28.06.13
 * Time: 12:20
 */

public class InsertDimension1 extends VoltProcedure {


    public final SQLStmt sql = new SQLStmt(
            "INSERT INTO d_t1 VALUES (?, ? ,?);"
    );

    public VoltTable[] run(String value, long time, String functionalId){
        voltQueueSQL(sql,value,time,functionalId);
        voltExecuteSQL();
        return null;
    }
}
