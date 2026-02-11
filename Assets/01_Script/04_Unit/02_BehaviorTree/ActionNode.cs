using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface INode
{
    public enum State { Failure, Success, Running }

    public abstract UniTask<State> Evaluate(CancellationToken ct);
}

public class ActionNode : INode
{
    Func<CancellationToken, UniTask<INode.State>> func;

    public ActionNode(Func<CancellationToken, UniTask<INode.State>> func) => this.func = func;
    public async UniTask<INode.State> Evaluate(CancellationToken ct)
    {
        if (func == null) 
            return INode.State.Failure;

        return await func.Invoke(ct);
    }
   
}
public abstract class CompositeNode : INode
{
    protected List<INode> children = new List<INode>();
    public void Add(INode node) => children.Add(node);
    public abstract UniTask<INode.State> Evaluate(CancellationToken token);
}

public class SelectorNode : CompositeNode
{

    public override async UniTask<INode.State> Evaluate(CancellationToken ct)
    {
        foreach (var node in children)
        {
            var state = await node.Evaluate(ct);
            if (state != INode.State.Failure) return state;
        }
        return INode.State.Failure;
    }
}

// Sequence: 모든 자식이 Success여야 성공 (AND 조건)
public class SequenceNode : CompositeNode
{
    public override async UniTask<INode.State> Evaluate(CancellationToken ct)
    {
        foreach (var node in children)
        {
            var state = await node.Evaluate(ct);
            if (state != INode.State.Success) return state;
        }
        return INode.State.Success;
    }
}
