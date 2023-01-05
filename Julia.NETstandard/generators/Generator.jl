include("Core/JPrimitive.jl")

project_root = pwd()

generate_primitives(project_root, "$project_root/src/Core", "$project_root/generated/Core")
