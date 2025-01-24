CREATE FUNCTION on_insert_user_function()
   RETURNS TRIGGER
   LANGUAGE PLPGSQL
AS
$$
BEGIN
   INSERT INTO UserGroupMembers(group_id, user_id) VALUES (1, NEW.user_id);
   RETURN NEW;
END;
$$;

CREATE TRIGGER on_insert_user_trigger
AFTER INSERT ON Users
FOR EACH ROW
EXECUTE FUNCTION on_insert_user_function();