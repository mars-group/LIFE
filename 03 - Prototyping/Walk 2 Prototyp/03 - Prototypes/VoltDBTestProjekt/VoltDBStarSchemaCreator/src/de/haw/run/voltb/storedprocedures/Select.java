package de.haw.run.voltb.storedprocedures;

import org.voltdb.*;

public class Select extends VoltProcedure {

    public final SQLStmt sql = new SQLStmt(
            "SELECT HELLO, WORLD FROM HELLOWORLD WHERE DIALECT = ?;"
    );

    public VoltTable[] run( String language) {
        voltQueueSQL( sql, language );
        return voltExecuteSQL();
    }
}