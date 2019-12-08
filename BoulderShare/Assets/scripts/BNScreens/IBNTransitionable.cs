interface IBNTransitionable
{
    void TransitionLerp(float t);
    void Ready();
    void Complete(bool isReverse);
}

