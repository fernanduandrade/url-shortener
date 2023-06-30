module Blog

open Microsoft.AspNetCore.Http
open Giraffe

[<CLIMutable>]
type BlogPost = {
    title: string
    content: string
}

type BlogDb() =
    let mutable allBlogPost: BlogPost list = []

    member this.GetAllPosts = fun() -> allBlogPost
    member this.AddPost (newPost: BlogPost) =
        allBlogPost <- (newPost::allBlogPost)
        allBlogPost

type BlogServiceTree = {
    getBlogDb: unit -> BlogDb
}

let getPostsHttpHandler (serviceTree: BlogServiceTree) =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        json (serviceTree.getBlogDb().GetAllPosts()) next ctx

let createPostHttpHandler (serviceTree: BlogServiceTree) =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! newPostJson = ctx.BindJsonAsync<BlogPost>()
            serviceTree.getBlogDb().AddPost(newPostJson) |> ignore
            return! json (newPostJson) next ctx
        }