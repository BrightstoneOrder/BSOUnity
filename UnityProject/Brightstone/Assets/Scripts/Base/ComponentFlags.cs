namespace Brightstone
{
    /** Flags detailing the state of the component.*/
    public enum ComponentFlags
    {
        /** Component has been initialized.*/
        CF_INITIALIZED,
        /** Component has been destroyed and is garbage.*/
        CF_GARBAGE,
        /** Component has been level initialized. */
        CF_LEVEL_INIT,
        /** Component is a manager*/
        CF_MANAGER,
        /** Component is a entity.*/
        CF_ACTOR,
        /** Component is registered for batch loading.*/
        CF_BATCH_LOAD_REGISTERED
    }

    /** typedef ComponentFlagsBitfield. */
    class ComponentFlagsBitfield : Bitfield<ComponentFlags> { }
}