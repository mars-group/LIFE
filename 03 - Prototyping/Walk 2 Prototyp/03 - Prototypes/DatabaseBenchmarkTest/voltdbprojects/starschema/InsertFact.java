import org.voltdb.SQLStmt;
import org.voltdb.VoltProcedure;
import org.voltdb.VoltTable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 28.06.13
 * Time: 12:28
 */
public class InsertFact extends VoltProcedure {

    public final SQLStmt sql = new SQLStmt(
            "INSERT INTO facts VALUES (?, ?, ? , ?, ?, ?, ?, ?, ?, ?, ?, ?);"
    );

    public VoltTable[] run(String fact, String functionalId){
        voltQueueSQL(sql,fact,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId,functionalId);
        voltExecuteSQL();
        return null;
    }
}
