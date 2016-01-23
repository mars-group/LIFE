package de.haw.run.GlobalTypes.Exceptions;

/**
 * Created with IntelliJ IDEA.
 * User: Chris
 * Date: 10.09.13
 * Time: 12:23
 * To change this template use File | Settings | File Templates.
 */
public class BaseURLNotAvailableException extends Throwable {
    public BaseURLNotAvailableException(Exception e) {
        super(e);
    }
}
