do
  local auto, assign

  function auto(tab, key)
    return setmetatable({}, {
            __index = auto,
            __newindex = assign,
            parent = tab,
            key = key
    })
  end

  local meta = {__index = auto}

  -- The if statement below prevents the table from being
  -- created if the value assigned is nil. This is, I think,
  -- technically correct but it might be desirable to use
  -- assignment to nil to force a table into existence.

  function assign(tab, key, val)
  -- if val ~= nil then
    local oldmt = getmetatable(tab)
    oldmt.parent[oldmt.key] = tab
    setmetatable(tab, meta)
    tab[key] = val
  -- end
  end

  function AutomagicTable()
    return setmetatable({}, meta)
  end
end

_G_mt = { __index = AutomagicTable }
setmetatable(_G, _G_mt)

--------------------------------------------------------------------------------

OpenNefia.Core.UI.Layer.TitleScreenLayer = 
{
   Window = {
      Title = "Starting Menu",
   },

   List = {
      Restore = { Text = "Restore an Adventurer" },
      Generate = { Text = "Generate an Adventurer" },
      Incarnate = { Text = "Incarnate an Adventurer" },
      About = { Text = "About" },
      Options = { Text = "Options" },
      Mods = { Text = "Mods" },
      Exit = { Text = "Exit" }
   },
}
